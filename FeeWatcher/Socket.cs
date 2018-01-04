using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

using Codeplex.Data;

namespace FeeWatcher {

    class Socket {

        const string URL = "wss://ws.blockchain.info/inv";

        private WebSocket socket;

        public void Connect() {
            socket = new WebSocket(URL);
            socket.OnMessage += OnMessage;
            socket.OnClose += OnClose;
            socket.OnError += OnError;
            socket.Connect();
        }

        public void Request(string message) {
            socket.Send(message);
        }

        protected virtual void OnMessage(object sender, MessageEventArgs e) {}

        protected virtual void OnClose(object sender, CloseEventArgs e) {}

        protected virtual void OnError(object sender, ErrorEventArgs e) {
            MessageBox.Show(e.Message);
        }

    }

    class TransactionSocket : Socket {

        LogStore logStore = new LogStore();

        MainWindow window;

        public TransactionSocket(MainWindow window) {
            this.window = window;
        }

        protected override void OnMessage(object sender, MessageEventArgs e) {

            try {

                dynamic parsedJason = DynamicJson.Parse(e.Data);
                WriteToWindow(TransactionConverter.ParseJsonTransaction(e.Data));

            } catch (Exception ex) {

                window.Dispatcher.Invoke(() => {
                    window.LogArea.Text = ex.Message + ex.StackTrace;
                });

            }

        }

        private void WriteToWindow(Transaction transaction) {

            window.Dispatcher.Invoke(() => {

                window.ScreenTransactionHash.Text = transaction.Hash;
                window.ScreenFeeBtc.Text = transaction.FeeBtc.ToString("F8") + "BTC"; ;
                window.ScreenFeeJpy.Text = (transaction.FeeBtc * Rate.BtcJpy).ToString("F0") + "円";
                window.ScreenSendBtc.Text = transaction.OutputValueBtc.ToString("F8") + "BTC";
                window.ScreenSendJpy.Text = (transaction.OutputValueBtc * Rate.BtcJpy).ToString("F0") + "円";
                window.ScreenFeeSatoshiPerByte.Text = Math.Round(transaction.FeeSatoshiPerByte).ToString() + "satoshi/byte";

                logStore.Add(transaction);
                logStore.Dump(window);

            });

        }

    }

    class SocketManager {

        Socket transactionSocket;

        public void ConnectTransaction(MainWindow window) {
            transactionSocket = new TransactionSocket(window);
            transactionSocket.Connect();
            transactionSocket.Request("{\"op\":\"unconfirmed_sub\"}");
        }
        
    }

}

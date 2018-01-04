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

using Codeplex.Data;

namespace FeeWatcher {

    public partial class MainWindow : Window {

        private SocketManager socketManager = new SocketManager();

        public MainWindow() {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize() {

            await Task.Run(() => {
                BitcoinPrice.Set();
                this.Dispatcher.Invoke(() => { this.Title += " BTC=" + Rate.BtcJpy.ToString() + "円"; });
                socketManager.ConnectTransaction(this);
            });

        }

    }

}

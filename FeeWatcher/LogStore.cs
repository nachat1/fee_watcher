using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeeWatcher {

    class LogStore {

        private List<Transaction> LogTransactions = new List<Transaction>();

        public void Dump(MainWindow window) {

            string dump = "";
            foreach (Transaction transaction in LogTransactions) {
                dump = string.Format(
                    "{0}      {1}BTC {2}円 {3}satoshi/byte",
                    transaction.Time,
                    transaction.FeeBtc.ToString("F8"),
                    String.Format("{0, 8}", (transaction.FeeBtc * Rate.BtcJpy).ToString("F0")),
                    String.Format("{0, 8}", (transaction.FeeSatoshiPerByte).ToString("F0"))
                ) + Environment.NewLine + dump;
            }

            window.LogArea.Text = dump;

        }

        public void Add(Transaction newTransaction) {

            LogTransactions.Add(newTransaction);

            if (LogTransactions.Count >= 80) {
                LogTransactions.RemoveAt(0);
            }

        }

    }

}

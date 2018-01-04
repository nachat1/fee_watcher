using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;

namespace FeeWatcher {

    class InOuntput {

        /// <summary>
        /// 使用済みか？
        /// </summary>
        public bool Spent {
            get;
            set;
        }

        /// <summary>
        /// トランザクションID
        /// </summary>
        public long TransactionIndex {
            get;
            set;
        }

        /// <summary>
        /// ???
        /// </summary>
        public string Type {
            get;
            set;
        }

        /// <summary>
        /// satoshi(1/10億btc)
        /// </summary>
        public long Value {
            get;
            set;
        }

        /// <summary>
        /// ???
        /// </summary>
        public long N {
            get;
            set;
        }

    }

    class Transaction {

        /// <summary>
        /// この時間を過ぎない限りブロックに取り込めない
        /// </summary>
        public long LockTime {
            get;
            set;
        }

        /// <summary>
        /// 現在は1
        /// </summary>
        public string Version {
            get;
            set;
        }

        /// <summary>
        /// ???
        /// </summary>
        public double Size {
            get;
            set;
        }

        /// <summary>
        /// ???時間
        /// </summary>
        public DateTime Time {
            get;
            set;
        }

        /// <summary>
        /// ハッシュ値
        /// </summary>
        public string Hash {
            get;
            set;
        }

        /// <summary>
        /// 内部管理用のトランザクションID
        /// </summary>
        public long TransactionIndex {
            get;
            set;
        }

        /// <summary>
        /// 入力トランザクション
        /// </summary>
        public List<InOuntput> Inputs = new List<InOuntput>();

        /// <summary>
        /// 出力トランザクション
        /// </summary>
        public List<InOuntput> Outputs = new List<InOuntput>();

        /// <summary>
        /// 送金量(satoshi)
        /// </summary>
        public long OutputValue {
            get {
                return Outputs.Sum(x => x.Value);
            }
        }

        /// <summary>
        /// 送金量(btc)
        /// </summary>
        public double OutputValueBtc {
            get {
                return OutputValue / 100000000.0;
            }
        }

        /// <summary>
        /// 手数料(satoshi)
        /// </summary>
        public long Fee {
            get {
                return Inputs.Sum(x => x.Value) - Outputs.Sum(x => x.Value);
            }
        }

        /// <summary>
        /// 手数料(btc換算)
        /// </summary>
        public double FeeBtc {
            get {
                return Fee / 100000000.0;
            }
        }

        public double FeeSatoshiPerByte {

            get {
                return Fee / Size;
            }

        }

    }

    static class TransactionConverter {

        /// <summary>
        /// Jsonからオブジェクトに変換する
        /// </summary>
        public static Transaction ParseJsonTransaction(string json) {

            dynamic parsedJason = DynamicJson.Parse(json).x;

            var transaction = new Transaction() {
                Hash = parsedJason.hash,
                LockTime = (long)parsedJason.lock_time,
                Size = parsedJason.size,
                Time = Utils.UnixtimeToDatetime((long)parsedJason.time),
                Version = parsedJason.ver.ToString(),
                TransactionIndex = (long)parsedJason.tx_index,
            };

            foreach (var input in parsedJason.inputs) {
                transaction.Inputs.Add(ParseJsonInputs(input.prev_out));
            }
            foreach (var output in parsedJason.@out) {
                transaction.Outputs.Add(ParseJsonInputs(output));
            }

            return transaction;

        }

        /// <summary>
        /// Jsonのinputsをオブジェクトに変換する
        /// </summary>
        private static InOuntput ParseJsonInputs(dynamic inputs) {

            return new InOuntput() {
                N = (long)inputs.n,
                Spent = bool.Parse(inputs.spent.ToString()),
                TransactionIndex = (long)inputs.tx_index,
                Type = inputs.type.ToString(),
                Value = (long)inputs.value,
            };

        }

    }

    class Transactions {

        private Dictionary<long, Transaction> transactions = new Dictionary<long /*transaction_id*/, Transaction>();

        public void Add(Transaction transaction) {
            lock (transactions) {
                transactions[transaction.TransactionIndex] = transaction;
            }
        }

        public void Remove(Transaction transaction) {
            lock (transactions) {
                transactions.Remove(transaction.TransactionIndex);
            }
        }

        public Transaction FindOrNull(long txid) {
            lock (transactions) {
                Transaction findTransaction;
                bool isExist = transactions.TryGetValue(txid, out findTransaction);
                return findTransaction;
            }
        }

    }

}

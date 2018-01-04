using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Codeplex.Data;

namespace FeeWatcher {

    static class Rate {

        public static double BtcJpy {
            get;
            set;
        }

    }

    class BitcoinPrice {

        static readonly Uri endpointUri = new Uri("https://api.bitflyer.jp");

        public static void Set() {
            Rate.BtcJpy = GetLast();
        }

        private static double GetLast() {

            var method = "GET";
            var path = "/v1/ticker";
            var query = "";
            var response = "";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(new HttpMethod(method), path + query)) {

                client.BaseAddress = endpointUri;
                var message = client.SendAsync(request).Result;
                response = message.Content.ReadAsStringAsync().Result;

            }

            try {

                return DynamicJson.Parse(response).ltp;

            } catch {

                throw new Exception("Faild to get btc price.");

            }

        }

    }

}
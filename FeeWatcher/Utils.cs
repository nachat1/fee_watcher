using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeeWatcher {

    static class Utils {

        /// <summary>
        /// Unixtimeの始まりの時間
        /// </summary>
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// unixtimeをdatetimeに変換する
        /// </summary>
        public static DateTime UnixtimeToDatetime(long unixtime) {
            return UNIX_EPOCH.AddSeconds(unixtime).ToLocalTime();
        }

    }

}

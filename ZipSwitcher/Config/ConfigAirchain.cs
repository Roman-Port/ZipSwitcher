using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZipSwitcher.Config
{
    class ConfigAirchain
    {
        /// <summary>
        /// Name of this airchain. Used for dialout.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// Root web URL of the Z/IP.
        /// </summary>
        [JsonProperty("zip_url")]
        public string ZipUrl { get; set; } = null;

        /// <summary>
        /// The filenane of the CSV file where logs will be placed. May be null.
        /// </summary>
        [JsonProperty("log_filename")]
        public string LogFilename { get; set; } = null;

        public void Verify()
        {
            if (Name == null)
                throw new ConfigVerifyException("Airchain name is not specified.");
            if (ZipUrl == null || !ZipUrl.StartsWith("http"))
                throw new ConfigVerifyException($"Airchain {Name}'s zip_url is either not specified or not an http URL.");
        }
    }
}

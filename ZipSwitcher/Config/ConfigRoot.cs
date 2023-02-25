using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZipSwitcher.Config
{
    class ConfigRoot
    {
        /// <summary>
        /// Name of this site. Used for dialout.
        /// </summary>
        [JsonProperty("site_name")]
        public string SiteName { get; set; } = null;

        /// <summary>
        /// How often the Z/IPs are refreshed, in milliseconds
        /// </summary>
        [JsonProperty("refresh_interval")]
        public int RefreshInterval { get; set; } = 30000;

        /// <summary>
        /// Each Z/IP this software watches.
        /// </summary>
        [JsonProperty("airchains")]
        public ConfigAirchain[] Airchains { get; set; } = new ConfigAirchain[0];

        /// <summary>
        /// The notification settings.
        /// </summary>
        [JsonProperty("dialout")]
        public ConfigDialout Dialout { get; set; } = null;

        public void Verify()
        {
            if (SiteName == null)
                throw new ConfigVerifyException("site_name is not specified in the configuration.");
            if (RefreshInterval < 1000)
                throw new ConfigVerifyException("refresh_interval must be >= 1000 ms.");
            if (Airchains == null || Airchains.Length == 0)
                throw new ConfigVerifyException("airchains must have at least one airchain.");
            foreach (var a in Airchains)
                a.Verify();
        }
    }
}

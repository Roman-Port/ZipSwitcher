using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZipSwitcher.Config
{
    class ConfigDialout
    {
        [JsonProperty("smtp_server")]
        public string SmtpServer { get; set; }

        [JsonProperty("smtp_port")]
        public int SmtpPort { get; set; }

        [JsonProperty("from_address")]
        public string FromAddress { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("targets")]
        public string[] Targets { get; set; }
    }
}

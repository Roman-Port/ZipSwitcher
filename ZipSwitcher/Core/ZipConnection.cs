using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZipSwitcher.Core
{
    class ZipConnection
    {
        public ZipConnection(Uri zipWebRoot)
        {
            //Set
            this.zipWebRoot = zipWebRoot;

            //Create client
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3);
        }

        private readonly Uri zipWebRoot;
        private readonly HttpClient client;

        public long Total { get; set; }
        public long Buffered { get; set; }
        public long Lost { get; set; }
        public long Concealed { get; set; }
        public long Dropped { get; set; }

        public async Task UpdateAsync()
        {
            //Fetch
            HttpResponseMessage response = await client.GetAsync(new Uri(zipWebRoot, "/json/connection_stats"));

            //Check
            response.EnsureSuccessStatusCode();
            string dataRaw = await response.Content.ReadAsStringAsync();

            //Decode as JSON
            NetPayload data = JsonConvert.DeserializeObject<NetPayload>(dataRaw);

            //Set
            Total = data.Rcv.Pkts.Total;
            Buffered = data.Rcv.Pkts.Buffered;
            Lost = data.Rcv.Pkts.Lost;
            Concealed = data.Rcv.Pkts.Concealed;
            Dropped = data.Rcv.Pkts.Dropped;
        }

        class NetPkts
        {
            [JsonProperty("total")]
            public long Total { get; set; }

            [JsonProperty("buffered")]
            public long Buffered { get; set; }

            [JsonProperty("lost")]
            public long Lost { get; set; }

            [JsonProperty("concealed")]
            public long Concealed { get; set; }

            [JsonProperty("dropped")]
            public long Dropped { get; set; }
        }

        class NetLink
        {
            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("codec")]
            public int Codec { get; set; }

            [JsonProperty("codecName")]
            public string CodecName { get; set; }

            [JsonProperty("sampleRate")]
            public int SampleRate { get; set; }

            [JsonProperty("channels")]
            public int Channels { get; set; }

            [JsonProperty("bitrate")]
            public int Bitrate { get; set; }

            [JsonProperty("minBuff")]
            public int MinBuff { get; set; }

            [JsonProperty("pkts")]
            public NetPkts Pkts { get; set; }
        }

        class NetPayload
        {
            [JsonProperty("rcv")]
            public NetLink Rcv { get; set; }

            [JsonProperty("xmt")]
            public NetLink Xmt { get; set; }
        }
    }
}

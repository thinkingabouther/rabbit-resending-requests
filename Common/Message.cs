using System;
using System.Text.Json.Serialization;

namespace Common
{
    public class Message
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("republish-count")]
        public int RepublishCount { get; set; } = 0;
    }
}
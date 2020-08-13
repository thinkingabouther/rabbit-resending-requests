using System;
using System.Text.Json.Serialization;

namespace Common
{
    public class Message
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
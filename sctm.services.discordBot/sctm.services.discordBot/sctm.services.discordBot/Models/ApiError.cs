using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace sctm.services.discordBot.Models
{
    public partial class ApiError
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        [JsonProperty("errors")]
        public Errors Errors { get; set; }
    }

    public partial class Errors
    {
        [JsonProperty("file")]
        public string[] File { get; set; }
    }
}

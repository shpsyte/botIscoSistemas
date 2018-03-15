using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotBlog.Models
{
    public class Customer
    {
        [JsonProperty("razao")]
        public string Razao { get; set; }
        [JsonProperty("fone")]
        public string Fone { get; set; }
        [JsonProperty("cgccpf")]
        public string CgcCpg { get; set; }
    }
}
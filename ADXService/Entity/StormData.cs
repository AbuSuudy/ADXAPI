using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADXService.Entity
{
    public class StormData
    {
        public DateTime DateTime { get; set; }
        [JsonPropertyName("state")]
        public required string State { get; set; }
        public long DamageCost { get; set; }
    }
}

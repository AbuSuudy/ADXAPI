using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADXService.Entity
{
    public class StormData
    {
        public DateTime DateTime { get; set; }
        public string State { get; set; }
        public long DamageCost { get; set; }
    }
}

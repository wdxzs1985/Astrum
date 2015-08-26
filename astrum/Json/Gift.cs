using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Gift
{
    class GiftInfo
    {
        public int total { get; set; }
        public int maxpage { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int restInBox { get; set; }
    }

    public class Enhance
    {
        public int strength { get; set; }
        public int limitbreak { get; set; }
        public int ability { get; set; }
    }

    public class GiftResult
    {
        public int total { get; set; }
        public int item { get; set; }
        public int lilu { get; set; }
        public int card { get; set; }
        public Enhance enhance { get; set; }
        public Dictionary<string, Instant> gacha { get; set; }
        public int practice { get; set; }
    }
    
    public class Instant
    {
        public int value { get; set; }
        public string type { get; set; }
    }
    
}

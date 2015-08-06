using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json
{
    public class Training
    {        
        public BaseInfo Base { get; set; }
        public long total { get; set; }
        public int maxpage { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        
    }

    public class BaseInfo
    {
        public string _id { get; set; }
        public string masterId { get; set; }
        public string name { get; set; }
        public string atr { get; set; }
        public string weapon { get; set; }
        public int cost { get; set; }
        public int level { get; set; }
        public long exp { get; set; }
        public long evolution { get; set; }
        public long rare { get; set; }
        public string race { get; set; }
        public int maxLevel { get; set; }
        public Growth growth { get; set; }
        public Voice voice { get; set; }
        public int limitbreak { get; set; }
        public long time { get; set; }
        public bool isMaxLevel { get; set; }
        public long abilityExp { get; set; }
        public int abilityLevel { get; set; }
        public int maxAbilityLevel { get; set; }
        public Ability ability { get; set; }
        public bool isMaxAbilitLevel { get; set; }
        public bool isMaxGrowth { get; set; }        
        public long total { get; set; }
        public long atk { get; set; }
        public long df { get; set; }
        public long mat { get; set; }
        public long mdf { get; set; }
        public bool inParty { get; set; }
    }

    public class Growth
    {
        public int exp { get; set; }
        public int ability { get; set; }
    }

    public class Voice
    {
        public string name { get; set; }
        public string message { get; set; }
    }

    public class Ability
    {
        public AbilityInfo front { get; set; }
        public AbilityInfo back { get; set; }
        public AbilityInfo support { get; set; }
        public AbilityInfo union { get; set; }
    }



    public class AbilityInfo
    {
        public string _id { get; set; }
        public string type { get; set; }
        public string atr { get; set; }
        public string cmd { get; set; }
        public int tp { get; set; }
        public Limit limit { get; set; }
        public string description { get; set; }
        public bool unique { get; set; }
        public bool open { get; set; }
        public string icon { get; set; }
    }

    public class Limit
    {
        public int count { get; set; }
    }
    
}

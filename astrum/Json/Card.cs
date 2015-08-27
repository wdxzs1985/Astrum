using Astrum.Json.Gacha;
using Astrum.Json.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Card
{

    public class CardInfo
    {
        public string _id { get; set; }
        public string masterId { get; set; }
        public string name { get; set; }
        public int rare { get; set; }
        public int level { get; set; }
        public int maxLevel { get; set; }
        public int abilityLevel { get; set; }
        public int maxAbilityLevel { get; set; }
        public int evolution { get; set; }
        public Md5 md5 { get; set; }                
        public string atr { get; set; }
        public string weapon { get; set; }
        public int cost { get; set; }        
        public long exp { get; set; }        
        public string race { get; set; }

        public Growth growth { get; set; }
        public Voice voice { get; set; }
        public int limitbreak { get; set; }
        public long time { get; set; }
        public bool isMaxLevel { get; set; }

        public long abilityExp { get; set; }      
        public Ability ability { get; set; }
        public bool isMaxAbilitLevel { get; set; }
        public bool isMaxGrowth { get; set; }
        public long total { get; set; }
        public long atk { get; set; }
        public long df { get; set; }
        public long mat { get; set; }
        public long mdf { get; set; }
        public bool inParty { get; set; }
        public int initial { get; set; }
        public int lilu { get; set; }

        public string DisplayName
        {
            get
            {
                string rare = "";
                switch (this.rare)
                {
                    case 4:
                        rare = "[SSR]";
                        break;
                    case 3:
                        rare = "[ SR]";
                        break;
                    case 2:
                        rare = "[  R]";
                        break;
                    case 1:
                        rare = "[  N]";
                        break;
                    default:
                        rare = "[???]";
                        break;
                }

                string evo = "";
                switch (this.evolution)
                {
                    case 1:
                        evo = " 觉醒";
                        break;
                    default:
                        break;
                }


                string inParty = "";
                if (this.inParty)
                {
                        inParty = " 编成中";
                }

                return string.Format("{0}{1} (Lv {2}{3}{4})", rare, name, level, evo, inParty);
            }
        }
    }

    public class Md5
    {
        public string image { get; set; }
        public string sd { get; set; }
        public string voice { get; set; }
    }

    public class CardSearchInfo
    {
        public int total { get; set; }
        public int maxpage { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public List<CardInfo> list { get; set; }
    }

    public class RaiseInfo : CardSearchInfo
    {
        public CardStock card { get; set; }
        public CardInfo @base { get; set; }       
        public Items items { get; set; }
    }

    public class Growth
    {
        public int exp { get; set; }
        public int ability { get; set; }
    }

    public class RaiseExecuteInfo
    {
        public List<CardInfo> removed { get; set; }
    }
    
    public class Items
    {
        public List<ItemInfo> exp { get; set; }
        public List<ItemInfo> ability { get; set; }
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

        //public Limit limit { get; set; }
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

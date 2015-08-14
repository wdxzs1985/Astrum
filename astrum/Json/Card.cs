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
        public Growth growth { get; set; }

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
                return string.Format("{0}{1} (Lv {2})", rare, name, level);
            }
        }
    }

    public class Md5
    {
        public string image { get; set; }
        public string sd { get; set; }
        public string voice { get; set; }
    }
    
    public class RaiseInfo
    {
        public CardStock card { get; set; }
        public CardInfo @base { get; set; }
        public int total { get; set; }
        public int maxpage { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public List<CardInfo> list { get; set; }        
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
    
}

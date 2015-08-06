using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Gacha
{
    public class GachaList
    {
        public Card card { get; set; }
        public Stock stock { get; set; }
        public string type { get; set; }
        public List<GachaInfo> list { get; set; }
    }

    public class Card
    {
        public int max { get; set; }
        public int value { get; set; }
    }

    public class GachaInfo
    {
        public string _id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public Price price { get; set; }
        public Enable enable { get; set; }
        public Sequence sequence { get; set; }
        public int stock { get; set; }
    }
    
    public class Sequence
    {
        public bool status { get; set; }
        public int value { get; set; }
    }
    
    public class Enable
    {
        public bool status { get; set; }
        public string type { get; set; }
    }
    
    public class Stock
    {
        public int coin { get; set; }
        public int gacha { get; set; }
        public Dictionary<string,int> ticket { get; set; }
    }


    public class GachaResult
    {
        public Card card { get; set; }
        public Stock stock { get; set; }
        public GachaInfo gacha { get; set; }

        public List<Item> list { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public string type { get; set; }
        public int value { get; set; }
        public int rare { get; set; }
    }

    public class Price
    {
        public string _id { get; set; }
        public string type { get; set; }
        public int value { get; set; }
        public string name { get; set; }
    }
}

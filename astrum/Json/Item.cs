using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Item
{
    public class ItemList
    {
        public List<ItemInfo> list { get; set; }
    }

    public class ItemInfo
    {
        public int stock { get; set; }
        public string _id { get; set; }
        public string calcType { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public double effect { get; set; }
        public string effectType { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int published { get; set; }
        public int rate { get; set; }
        public string type { get; set; }
        public bool isGacha { get; set; }
        public int available { get; set; }
    }

    public class UseItemResult
    {
        public string _id { get; set; }
        public ValueComparer stock { get; set; }
        public ValueComparer value { get; set; }
    }

    public class ValueComparer
    {
        public int after { get; set; }
        public int before { get; set; }
    }

    public class Giftbox
    {
        public int limited { get; set; }
    }
}

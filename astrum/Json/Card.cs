using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json
{

    public class Card
    {
        public string _id { get; set; }
        public string name { get; set; }
        public long rare { get; set; }
        public long evolution { get; set; }
        public Md5 md5 { get; set; }
    }

    public class Md5
    {
        public string image { get; set; }
        public string sd { get; set; }
        public string voice { get; set; }
    }
}

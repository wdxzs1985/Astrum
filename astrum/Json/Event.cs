using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Event
{

    public class EventStatus
    {
        public List<Event> list { get; set; }
    }

    public class Event
    {
        public string _id { get; set; }
        public string type { get; set; }
        public bool status { get; set; }
        public Term term { get; set; }
    }


    public class Term
    {
        public long start;

        public long end;
    }
}

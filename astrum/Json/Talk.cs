using Astrum.Json.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Talk
{
    public class TalkListInfo
    {
        public int total { get; set; }
        public int maxpage { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public List<TalkInfo> list { get; set; }
    }

    public class TalkInfo
    {
        public string _id { get; set; }
        public long time { get; set; }
        public int unread { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public List<Talker> group { get; set; }
    }

    public class Talker
    {
        public string cardId { get; set; }
        public string partnerId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string atr { get; set; }
        public string nickName { get; set; }
        public Md5 md5 { get; set; }
    }
}

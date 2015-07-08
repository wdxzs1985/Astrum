using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Astrum.Json.Raid;

namespace Astrum.Json.Mypage
{
    public class Guild
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string rank { get; set; }
        public long memberCount { get; set; }
        public long memberSpace { get; set; }
        public bool isRegisterdCenter { get; set; }
    }

    public class Status
    {
        public long level { get; set; }
        public long exp_value { get; set; }
        public long exp_min { get; set; }
        public long exp_max { get; set; }
        public long stamina_max { get; set; }
        public long stamina_time { get; set; }
        public long stamina_value { get; set; }
        public long stamina_maxTime { get; set; }
        public long bp_value { get; set; }
        public long bp_max { get; set; }
        public long tp_value { get; set; }
        public long tp_max { get; set; }
        public long gacha { get; set; }
        public long lilu { get; set; }
        public long card_quantity { get; set; }
        public long card_max { get; set; }
        public long total { get; set; }
        public long atk { get; set; }
        public long df { get; set; }
        public long mat { get; set; }
        public long mdf { get; set; }
        public string name { get; set; }
    }

    public class Quest
    {
        public string _id { get; set; }
    }


    public class Link
    {
        public Quest quest { get; set; }
        //public RaidBattleInfo raid { get; set; }
    }



    public class MypageInfo
    {
        public Status status { get; set; }
        public long total { get; set; }
        public List<Card> cardList { get; set; }
        public Guild guild { get; set; }
        public Link link { get; set; }
    }

}

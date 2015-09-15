using Astrum.Json.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Astrum.Json.Mypage
{
    public class Guild
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string rank { get; set; }
        public int memberCount { get; set; }
        public int memberSpace { get; set; }
        public bool isRegisterdCenter { get; set; }
    }

    public class Status
    {
        public int level { get; set; }
        public long exp_value { get; set; }
        public long exp_min { get; set; }
        public long exp_max { get; set; }
        public int stamina_max { get; set; }
        //public long stamina_time { get; set; }
        public int stamina_value { get; set; }
        //public long stamina_maxTime { get; set; }
        public int bp_value { get; set; }
        public int bp_max { get; set; }
        public int tp_value { get; set; }
        public int tp_max { get; set; }
        public long gacha { get; set; }
        public long lilu { get; set; }
        public int card_quantity { get; set; }
        public int card_max { get; set; }
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

    public class MypageInfo
    {
        public Status status { get; set; }
        public long total { get; set; }
        //public List<CardInfo> cardList { get; set; }
        public Guild guild { get; set; }
        //public Link link { get; set; }
        public bool talk { get; set; }

        public LoginBonusInfo loginBonus { get; set; }
    }

    public class LoginBonusInfo
    {
        public bool basic { get; set; }
        public bool @event { get; set; }
        public bool longLogin { get; set; }
    }

    public class ProfileInfo
    {
        public string _id { get; set; }
        public User user { get; set; }
        //public List<AppealDeck> appealDeck { get; set; }
        public bool isMe { get; set; }
        //public Invite invite { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public long accessTime { get; set; }
        public int level { get; set; }
        public int total { get; set; }
        public Guild guild { get; set; }
        public CardInfo leader { get; set; }
    }
    
}

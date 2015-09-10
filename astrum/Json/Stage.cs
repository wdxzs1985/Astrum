using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Astrum.Json.Mypage;
using Astrum.Json.Raid;
using Astrum.Json.Item;
using Astrum.Json.Event;

namespace Astrum.Json.Stage
{
    public class StageInfo
    {
        public string _id { get; set; }
        public string name { get; set; }
        public object drop { get; set; }
        public int progress { get; set; }
        public int order { get; set; }
        public int stage { get; set; }
        public bool staminaEmpty { get; set; }
        public bool stageClear { get; set; }
        public bool isBossStage { get; set; }

        public RaidBattleInfo raid { get; set; }
        public RaidBattleInfo furyraid { get; set; }
        public RaidBattleInfo limitedraid { get; set; }
        public RaidBattleInfo breeding { get; set; }

        public Status status { get; set; }

        public StageInfo initial { get; set; }
        public StageInfo nextStage { get; set; }

        public List<ItemInfo> items { get; set; }
    }

    public class Stamina
    {
        public int value { get; set; }
        public int max { get; set; }
    }

    public class Exp
    {
        public int value { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class Bp
    {
        public int value { get; set; }
        public int max { get; set; }
    }

    public class Tp
    {
        public int value { get; set; }
        public int max { get; set; }
    }

    public class Status
    {
        public int level { get; set; }
        public Stamina stamina { get; set; }
        public Exp exp { get; set; }
        public Bp bp { get; set; }
        public Tp tp { get; set; }
        public int lilu { get; set; }
        public int gacha { get; set; }
        public StageRaidInfo raid { get; set; }
        public LimitedRaidEventInfo limitedraid { get; set; }
        public StageRaidInfo furyraid { get; set; }
        public StageRaidInfo breeding { get; set; }
        public bool guild { get; set; }
    }

    public class StageRaidInfo
    {
        public string _id { get; set; }

        public string eventId { get; set; }

        public RaidBattleInfo find { get; set; }

        public RaidBattleInfo rescue { get; set; }

        public Fever fever { get; set; }
    }

    public class AreaBossInfo
    {
        public string _id { get; set; }
        public string name { get; set; }
        public int hp { get; set; }
        public string areaId { get; set; }
        public string areaName { get; set; }
        public int totalDamage { get; set; }
        public Status status { get; set; }
        public bool isLastBoss { get; set; }
    }

    public class MapInfo
    {
        public string _id { get; set; }
        public string name { get; set; }
        public List<AreaInfo> list { get; set; }
    }


    public class ZoneInfo
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class AreaInfo{
        public string _id { get; set; }
        public string name { get; set; }
        public bool isNew { get; set; }
        public bool isClear { get; set; }
        public int status { get; set; }
        public int stock { get; set; }
        public int order { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Astrum.Json.Mypage;
using Astrum.Json.Raid;

namespace Astrum.Json.Stage
{
    class Stage
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
        public object limitedraid { get; set; }
        public object furyraid { get; set; }

        public Status status { get; set; }
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

    public class Limitedraid
    {
    }

    public class Furyraid
    {
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
        //public RaidInfo raid { get; set; }
        public Limitedraid limitedraid { get; set; }
        public Furyraid furyraid { get; set; }
        public bool guild { get; set; }
    }
}

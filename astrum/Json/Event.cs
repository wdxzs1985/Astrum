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
    }


    public class GuildBattleLobbyInfo
    {
        public string status { get; set; }
        public bool available { get; set; }
        public List<Schedule> schedule { get; set; }
    }

    public class Schedule
    {
        public string _id { get; set; }
        public string wsId { get; set; }
        public Guild guild { get; set; }
        public bool isPickup { get; set; }
        public string status { get; set; }
    }

    public class Guild
    {
        public string _id { get; set; }
        public User user { get; set; }
        public User opponent { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public string name { get; set; }
        public int point { get; set; }
    }

    public class GuildBattleInfo
    {
        public List<Guild> guilds { get; set; }
        public GuildBattleStatus status { get; set; }
        public Stamp stamp { get; set; }
    }

    public class Stamp
    {
        public bool status { get; set; }
    }

    public class GuildBattleStatus
    {
        public User user { get; set; }
        public Guild guild { get; set; }
        public string position { get; set; }
        public Hp hp { get; set; }
        public Tp tp { get; set; }
        public int point { get; set; }
        public Total total { get; set; }
        public Counter counter { get; set; }
    }

    public class Hp
    {
        public int max { get; set; }
        public int value { get; set; }
    }

    public class Tp
    {
        public int interval { get; set; }
        public long time { get; set; }
        public int unitValue { get; set; }
        public int value { get; set; }
        public int max { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public int buff { get; set; }
    }

    public class Counter
    {
        public SkillCounter attack { get; set; }
        public SkillCounter special { get; set; }
        public SkillCounter recovery { get; set; }
        public SkillCounter union { get; set; }
        public SkillCounter burst { get; set; }
        public SkillCounter remote { get; set; }
        public SkillCounter yell { get; set; }
    }


    public class SkillCounter
    {
        public int count { get; set; }
        public bool infinity { get; set; }
        public bool availableStun { get; set; }
        public bool useUnion { get; set; }
    }

    public class GuildBattleCmdInfo
    {
        public string type { get; set; }
        public List<Cmd> cmd { get; set; }
        public SkillCounter counter { get; set; }
        public GuildBattleStatus status { get; set; }
    }

    public class Cmd
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int tp { get; set; }
        public string atr { get; set; }
        public CmdStatus status { get; set; }
        public bool isDefault { get; set; }
        public bool infinity { get; set; }
        public bool availableStun { get; set; }
        public Card card { get; set; }
    }

    public class CmdStatus
    {
        public bool available { get; set; }
    }

    public class TpInfo
    {
        public TpStatus normal { get; set; }
        public TpStatus chat { get; set; }
        public TpStatus roulette { get; set; }
        //public TpStatus status { get; set; }
    }

    public class TpStatus
    {
        public bool available { get; set; }
    }

    public class Roulette
    {
        public bool available { get; set; }
        public int initialPosition { get; set; }
        public List<int> order { get; set; }
    }
}

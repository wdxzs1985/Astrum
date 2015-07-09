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
        //public Status status { get; set; }
        public Stamp stamp { get; set; }
        public Event @event { get; set; }
        public List<string> activeUsers { get; set; }
    }

    public class Stamp
    {
        public bool status { get; set; }
    }
}

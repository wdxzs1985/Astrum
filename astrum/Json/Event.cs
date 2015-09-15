using Astrum.Json.Card;
using Astrum.Json.Raid;
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

    public class RaidEventInfo
    {
        public string _id { get; set; }

        public Rewards rewards { get; set; }

    }

    public class FuryRaidEventInfo
    {
        public string name { get; set; }

        //public RaidBattleInfo rare { get; set; }

        public string eventId { get; set; }

        public Fever fever { get; set; }

        public TotalRewards totalRewards { get; set; }
    }
    
    public class LimitedRaidEventInfo
    {
        public string _id { get; set; }

        public string eventId { get; set; }

        public Fever fever { get; set; }

        public bool isNew { get; set; }

        public RaidBattleInfo target { get; set; }
    }

    public class Fever
    {
        public int progress { get; set; }

        public double effect { get; set; }

        public object gachaTicket { get; set; }

        public object breedingPoint { get; set; }
    }

    public class BreedingEventInfo
    {
        public string _id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string breedingPointName { get; set; }

        public TotalRewards totalRewards { get; set; }
        public List<Partner> partners { get; set; }

        public int breedingPoint { get; set; }
        public int exchangePoint { get; set; }

        public RaidBattleInfo target { get; set; }
    }

    public class TotalRewards
    {
        public Rewards guild { get; set; }
        public Rewards user { get; set; }
    }

    public class Rewards
    {
        public int total { get; set; }
        public bool isComplete { get; set; }
        public Next next { get; set; }
    }

    public class Next
    {
        public string _id { get; set; }
        public string type { get; set; }
        public int value { get; set; }
        public int title { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int requirement { get; set; }
    }

    public class Partner
    {
        public string breedingId { get; set; }
        public string eventId { get; set; }
        public bool canBreed { get; set; }
        public string cardId { get; set; }
        public int breedingLevel { get; set; }
        public int maxBreedingLevel { get; set; }
        public CardInfo card { get; set; }
        public Next nextBreedReward { get; set; }
        
    }

    public class RankingInfo
    {
        public string eventId { get; set; }

        public int point { get; set; }

        public int ranking { get; set; }
    }
}

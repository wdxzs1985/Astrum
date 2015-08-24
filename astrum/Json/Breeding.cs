using Astrum.Json.Card;
using Astrum.Json.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Breeding
{
    public class BreedingInfo
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
    
}

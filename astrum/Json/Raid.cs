using Astrum.Json.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json.Raid
{
    public class RaidInfo
    {
        public RaidBattleInfo find { get; set; }

        public RaidBattleList rescue { get; set; }

        public bool isFullRewards { get; set; }
    }

    public class RaidBattleList
    {
        public List<RaidBattleInfo> list { get; set; }
    }

    public class RaidBattleInfo
    {
        public string _id { get; set; }
        public string eventId { get; set; }

        public string bossId { get; set; }
        public string type { get; set; }

        public string name { get; set; }
        public int level { get; set; }
        public int rare { get; set; }

        public int bpValue { get; set; }
        public int bpMax { get; set; }
        public int combo { get; set; }
        public bool canCombo { get; set; }
        public int hp { get; set; }
        public int totalDamage { get; set; }
        public bool isWin { get; set; }
        public bool isPlaying { get; set; }
        public bool isLose { get; set; }
        public bool isNew { get; set; }
        public int joinNum { get; set; }

        public RescueInfo rescue { get; set; }

        public string Boss
        {
            get
            {
                string type = "";
                switch (this.rare)
                {
                    case 1:
                        type = "初级魔星兽";
                        break;
                    case 2:
                        type = "中级魔星兽";
                        break;
                    case 3:
                        type = "上级魔星兽";
                        break;
                    case 4:
                        type = "星兽王";
                        break;
                    default:
                        type = "魔星兽";
                        break;
                }

                return String.Format("{0}({1} L{2})出现了", type, name, level);
            }
        }
    }

    public class RescueInfo
    {
        public bool use { get; set; }
    }

    public class BossBattleResultInfo
    {
        public Init init { get; set; }
        public Result result { get; set; }

        public bool isEnd { get; set; }
    }

    public class Init
    {
        public DeckInfo boss { get; set; }
        public string bpType { get; set; }
        public List<DeckInfo> cards { get; set; }
    }

    public class Result
    {
        public DeckInfo afterBoss { get; set; }
        public Dictionary<string, DeckInfo> afterDeck { get; set; }

        public string resultType { get; set; }
    }

    public class DeckInfo
    {
        public int maxHp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
    }

    public class FuryRaidInfo
    {
        public RaidBattleList find { get; set; }

        public RaidBattleList rescue { get; set; }

        public string eventId { get; set; }
    }
}

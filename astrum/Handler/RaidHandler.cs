using Astrum.Http;
using Astrum.Json.Raid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    public class RaidHandler
    {
        private AstrumClient _client;

        public RaidHandler(AstrumClient client)
        {
            _client = client;
        }

        public void Run()
        {
            var raidInfo = RaidInfo();

            if (raidInfo.find != null)
            {
                var battleInfo = raidInfo.find;
                var loop = battleInfo.isNew || _client.ViewModel.CanFullAttack;
                while (loop)
                {
                    loop = RaidBattle(battleInfo._id);
                }
            }

            if (raidInfo.rescue != null)
            {
                foreach (var battleInfo in raidInfo.rescue.list)
                {
                    var loop = battleInfo.isNew || _client.ViewModel.CanFullAttack;
                    while (loop)
                    {
                        loop = RaidBattle(battleInfo._id);
                    }
                }
            }
        }

        public RaidInfo RaidInfo()
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/raid");
            var raidInfo = JsonConvert.DeserializeObject<RaidInfo>(result);
            _client.DelayShort();

            return raidInfo;
        }


        public bool RaidBattle(string raidId)
        {
            var battleInfo = BattleInfo(raidId);
            if (battleInfo.isPlaying)
            {
                if (battleInfo.isNew)
                {
                    RaidBattleAttack(battleInfo._id, AstrumClient.FIRST);
                    return true;
                }

                if (battleInfo.rescue.use)
                {
                    RaidBattleRescue(battleInfo._id);
                    return true;
                }

                if (_client.ViewModel.CanAttack)
                {
                    var hp = battleInfo.hp - battleInfo.totalDamage;
                    bool useFullAttack = hp > _client.ViewModel.EasyBossDamage;

                    var attackType = useFullAttack ? AstrumClient.FULL : AstrumClient.NORMAL;
                    var needBp = useFullAttack ? AstrumClient.BP_FULL : AstrumClient.BP_NORMAL;

                    if (_client.ViewModel.BpValue >= needBp)
                    {
                        RaidBattleAttack(battleInfo._id, attackType);
                        return true;
                    }
                }
            }
            else
            {
                RaidBattleResult(raidId);
            }
            return false;
        }


        private RaidBattleInfo BattleInfo(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/raid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            InfoPrinter.PrintRaidBattleInfo(battleInfo, _client.ViewModel);
            InfoUpdater.UpdateBpAfterRaidBattle(battleInfo, _client.ViewModel);

            _client.DelayShort();

            return battleInfo;
        }
        
        private void RaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/raid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);
            InfoUpdater.UpdateBattleDamage(battleResultInfo, _client.ViewModel);

            _client.DelayLong();
        }

        private void RaidBattleRescue(string raidId)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/raid/battlerescue", values);
            _client.DelayShort();
        }

        private void RaidBattleResult(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/raid/battleresult?_id=" + Uri.EscapeDataString(raidId));
            //RaidBattleInfo battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            _client.GetXHR("http://astrum.amebagames.com/_/raid/summary");
            _client.DelayShort();
        }

    }
}

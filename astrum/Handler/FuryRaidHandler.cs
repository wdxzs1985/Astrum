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
    class FuryRaidHandler
    {

        private AstrumClient _client;

        public FuryRaidHandler(AstrumClient client)
        {
            _client = client;
        }

        public void Run()
        {
            FuryRaidInfo raidInfo = FuryRaidInfo();
            _client.ViewModel.FeverProgress = raidInfo.fever.progress;
            //_client.ViewModel.Fever = raidInfo.fever.progress == 100;

            raidInfo = FuryRaidBoss();
            if (raidInfo.find != null)
            {
                //ViewModel.FuryRaidFindList = raidInfo.find.list;
                foreach (var battleInfo in raidInfo.find.list)
                {
                    bool loop = battleInfo.rare == 4 || (!_client.ViewModel.Fever && (battleInfo.isNew || _client.ViewModel.CanFullAttack));

                    while (loop)
                    {
                        loop = FuryRaidBattle(battleInfo._id);
                    }

                }
            }

            if (raidInfo.rescue != null)
            {
                foreach (var battleInfo in raidInfo.rescue.list)
                {
                    var loop = battleInfo.isNew && !_client.ViewModel.Fever;
                    while (loop)
                    {
                        loop = FuryRaidBattle(battleInfo._id);
                    }
                }
            }
        }


        public FuryRaidInfo FuryRaidInfo()
        {
            _client.Access("furyraid");

            var eventId = _client.ViewModel.FuryRaidEventId;
            string result = _client.GetXHR("http://astrum.amebagames.com/_/event/furyraid?_id=" + Uri.EscapeDataString(eventId));
            FuryRaidInfo raidInfo = JsonConvert.DeserializeObject<FuryRaidInfo>(result);

            _client.DelayShort();
            return raidInfo;
        }

        public FuryRaidInfo FuryRaidBoss()
        {
            var eventId = _client.ViewModel.FuryRaidEventId;
            string result = _client.GetXHR("http://astrum.amebagames.com/_/event/furyraid/bosses?_id=" + Uri.EscapeDataString(eventId));
            FuryRaidInfo raidInfo = JsonConvert.DeserializeObject<FuryRaidInfo>(result);

            _client.DelayShort();
            return raidInfo;
        }

        public bool FuryRaidBattle(string raidId)
        {
            var battleInfo = FuryBattleInfo(raidId);

            if (battleInfo.isPlaying)
            {
                if (battleInfo.isNew)
                {
                    var attackType = AstrumClient.FIRST;
                    if (AstrumClient.RESCUE.Equals(battleInfo.type))
                    {
                        attackType = AstrumClient.RESCUE;
                    }

                    FuryRaidBattleAttack(battleInfo._id, attackType);
                    return true;
                }

                if (battleInfo.rescue.use)
                {
                    FuryRaidBattleRescue(battleInfo._id);
                }

                if (AstrumClient.FIND.Equals(battleInfo.type))
                {
                    var hp = battleInfo.hp - battleInfo.totalDamage;
                    bool useFullAttack = hp > _client.ViewModel.EasyBossDamage;

                    var attackType = useFullAttack ? AstrumClient.FULL : AstrumClient.NORMAL;
                    var needBp = useFullAttack ? AstrumClient.BP_FULL : AstrumClient.BP_NORMAL;

                    if (battleInfo.rare == 4)
                    {
                        int quantity = needBp - _client.ViewModel.BpValue;
                        if (quantity > 0 && quantity <= _client.ViewModel.CanUseBpQuantity)
                        {
                            _client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_MINI_BP, quantity);
                        }
                    }

                    if (_client.ViewModel.BpValue >= needBp)
                    {
                        FuryRaidBattleAttack(battleInfo._id, attackType);
                        return true;
                    }
                }
            }
            else
            {
                FuryRaidBattleResult(raidId);
            }
            return false;
        }

        private RaidBattleInfo FuryBattleInfo(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/event/furyraid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            InfoPrinter.PrintRaidBattleInfo(battleInfo, _client.ViewModel);
            InfoUpdater.UpdateBpAfterRaidBattle(battleInfo, _client.ViewModel);

            _client.DelayShort();
            return battleInfo;
        }

        private void FuryRaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/event/furyraid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);
            InfoUpdater.UpdateBattleDamage(battleResultInfo, _client.ViewModel);

            _client.DelayLong();
        }

        private void FuryRaidBattleRescue(string raidId)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/event/furyraid/battlerescue", values);
            _client.DelayShort();
        }

        private void FuryRaidBattleResult(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/event/furyraid/battleresult?_id=" + Uri.EscapeDataString(raidId));
            RaidBattleInfo battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            var eventId = battleInfo.eventId;
            _client.GetXHR("http://astrum.amebagames.com/_/event/furyraid/summary?_id=" + Uri.EscapeDataString(eventId));
        }
    }
}

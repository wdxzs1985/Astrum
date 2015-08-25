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
    class LimitedRaidHandler
    {
        private AstrumClient _client;

        public LimitedRaidHandler(AstrumClient client)
        {
            _client = client;
        }


        public void Run()
        {
            var raidInfo = LimitedRaidInfo();

            var loop = raidInfo.target != null && _client.ViewModel.CanFullAttackForEvent;

            while (loop)
            {
                loop = LimitedRaidBattle(raidInfo.target._id);
            }
        }


        public LimitedRaidInfo LimitedRaidInfo()
        {
            var eventId = _client.ViewModel.LimitedRaidEventId;
            string result = _client.GetXHR("http://astrum.amebagames.com/_/event/limitedraid?_id=" + Uri.EscapeDataString(eventId));
            var raidInfo = JsonConvert.DeserializeObject<LimitedRaidInfo>(result);

            _client.ViewModel.Fever = raidInfo.fever.gachaTicket != null;

            _client.DelayShort();
            return raidInfo;
        }

        public bool LimitedRaidBattle(string raidId)
        {
            var battleInfo = LimitedRaidBattleInfo(raidId);

            if (battleInfo.isPlaying)
            {
                var hp = battleInfo.hp - battleInfo.totalDamage;

                var attackType = hp > AstrumClient.EASY_BOSS_HP ? AstrumClient.FULL : AstrumClient.NORMAL;
                var needBp = hp > AstrumClient.EASY_BOSS_HP ? AstrumClient.BP_FULL : AstrumClient.BP_NORMAL;

                if (_client.ViewModel.Fever)
                {
                    int quantity = needBp - _client.ViewModel.BpValue;
                    if (quantity > 0 && quantity <= _client.ViewModel.CanUseBpQuantity)
                    {
                        _client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_MINI_BP, quantity);
                    }
                }

                if (_client.ViewModel.BpValue >= needBp)
                {
                    LimitedRaidBattleAttack(battleInfo._id, attackType);
                    return true;
                }
            }
            return false;
        }

        private RaidBattleInfo LimitedRaidBattleInfo(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/limitedraid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            InfoPrinter.PrintRaidBattleInfo(battleInfo, _client.ViewModel);

            InfoUpdater.UpdateBpAfterRaidBattle(battleInfo, _client.ViewModel);

            _client.DelayShort();

            return battleInfo;
        }


        private void LimitedRaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/limitedraid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);
            _client.DelayShort();

        }
    }
}

using Astrum.Http;
using Astrum.Json.Event;
using Astrum.Json.Raid;
using Astrum.Json.Stage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class BreedingHandler
    {
        private AstrumClient _client;

        public BreedingHandler(AstrumClient client)
        {
            _client = client;
        }


        public void Run()
        {
            _client.Access("breeding");

            var breedingInfo = BreedingInfo();

            var stage = EnterBreedingStage();
            var areaId = stage._id;

            while (_client.ViewModel.IsRunning)
            {
                if (stage.isBossStage)
                {
                    BreedingAreaBossBattle(areaId);
                    return;
                }
                else if (stage.stageClear && stage.nextStage.isBossStage)
                {
                    stage = ForwardBreedingStage(areaId);
                    _client.RaiseNotificationEvent("area boss", AstrumClient.DELAY_LONG);
                    BreedingAreaBossBattle(areaId);
                    return;
                }
                else
                {
                    var breedingRaidId = stage.status.breeding._id;
                    if (breedingRaidId != null)
                    {
                        _client.ViewModel.IsBreedingRaid = true;
                        if (_client.ViewModel.CanFullAttackForEvent)
                        {
                            BreedingRaid(breedingRaidId);
                            return;
                        }
                    }
                    else
                    {
                        _client.ViewModel.IsBreedingRaid = false;
                    }

                    bool isBpFull = _client.ViewModel.BpValue >= AstrumClient.BP_FULL;
                    bool isFever = _client.ViewModel.Fever;

                    if (!isBpFull && !isFever)
                    {
                        _client.ViewModel.IsStaminaEmpty = true;
                    }
                    else
                    {
                        _client.ViewModel.IsStaminaEmpty = false;
                    }

                    if (_client.ViewModel.IsStaminaEmpty)
                    {
                        bool staminaGreaterThanKeep = _client.ViewModel.StaminaValue >= _client.ViewModel.KeepStamina;
                        bool staminaGreaterThanExp = _client.ViewModel.StaminaValue >= (_client.ViewModel.ExpMax - _client.ViewModel.ExpValue);

                        if (staminaGreaterThanKeep || staminaGreaterThanExp)
                        {
                            _client.ViewModel.IsStaminaEmpty = false;
                        }
                        else
                        {
                            return;
                        }
                    }


                    if (stage.staminaEmpty)
                    {
                        if (stage.items != null && _client.ViewModel.ExpMax - _client.ViewModel.ExpValue > 100)
                        {
                            var item = stage.items.Find(e => AstrumClient.INSTANT_HALF_STAMINA.Equals(e._id));
                            if (item.stock > _client.ViewModel.MinStaminaStock && _client.ViewModel.Fever)
                            {
                                _client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_HALF_STAMINA, 1);
                                return;
                            }
                            else
                            {
                                _client.ViewModel.IsStaminaEmpty = true;
                                return;
                            }
                        }
                        else
                        {
                            _client.ViewModel.IsStaminaEmpty = true;
                            return;
                        }
                    }
                    //forward                   
                    stage = ForwardBreedingStage(areaId);
                }
            }
        }


        private BreedingEventInfo BreedingInfo()
        {
            var url = string.Format("http://astrum.amebagames.com/_/event/breeding?_id={0}", Uri.EscapeDataString(_client.ViewModel.BreedingEventId));
            var result = _client.GetXHR(url);
            var info = JsonConvert.DeserializeObject<BreedingEventInfo>(result);

            InfoPrinter.PrintBreedingEventInfo(info, _client.ViewModel);

            _client.DelayShort();
            return info;
        }

        public StageInfo EnterBreedingStage()
        {
            var areaId = "breeding0001-1";
            //MapInfo map = BreedingMap();
            //var areaId = map.list[0]._id;

            var url = string.Format("http://astrum.amebagames.com/_/breeding/stage?areaId={0}&eventId={1}", areaId, Uri.EscapeDataString(_client.ViewModel.BreedingEventId));
            var result = _client.GetXHR(url);

            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);
            InfoUpdater.UpdateStageView(stage.initial, _client.ViewModel);
            
            _client.DelayShort();
            return stage;
        }


        private StageInfo ForwardBreedingStage(string areaId)
        {
            var values = new Dictionary<string, object>
                {
                   { "areaId", areaId },
                   { "eventId", _client.ViewModel.BreedingEventId }
                };
            var result = _client.PostXHR("http://astrum.amebagames.com/_/breeding/stage", values);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);

            var feverBefore = _client.ViewModel.Fever;
            InfoUpdater.UpdateStageView(stage, _client.ViewModel);
            if(_client.ViewModel.Fever && feverBefore != _client.ViewModel.Fever)
            {
                _client.RaiseNotificationEvent("Fever start", AstrumClient.SECOND * 60);
            }

            _client.DelayShort();
            return stage;
        }

        private MapInfo BreedingMap()
        {
            var url = string.Format("http://astrum.amebagames.com/_/event/map?eventId={0}", Uri.EscapeDataString(_client.ViewModel.BreedingEventId));
            var result = _client.GetXHR(url);

            return JsonConvert.DeserializeObject<MapInfo>(result);
        }

        private void BreedingRaid(string raidId)
        {
            var loop = _client.ViewModel.CanFullAttackForEvent;
            while (loop)
            {
                loop = BreedingRaidBattle(raidId);
            }
        }

        public bool BreedingRaidBattle(string raidId)
        {
            var battleInfo = BreedingRaidBattleInfo(raidId);

            if (battleInfo.isPlaying)
            {
                var hp = battleInfo.hp - battleInfo.totalDamage;
                bool useFullAttack = hp > _client.ViewModel.EasyBossDamage;

                var attackType = useFullAttack ? AstrumClient.FULL : AstrumClient.NORMAL;
                var needBp = useFullAttack ? AstrumClient.BP_FULL : AstrumClient.BP_NORMAL;

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
                    BreedingRaidBattleAttack(battleInfo._id, attackType);
                    return true;
                }
            }
            return false;
        }

        private RaidBattleInfo BreedingRaidBattleInfo(string raidId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/breeding/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            InfoPrinter.PrintRaidBattleInfo(battleInfo, _client.ViewModel);
            InfoUpdater.UpdateBpAfterRaidBattle(battleInfo, _client.ViewModel);

            _client.DelayShort();
            return battleInfo;
        }


        private void BreedingRaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/breeding/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);
            InfoUpdater.UpdateBattleDamage(battleResultInfo, _client.ViewModel);

            _client.DelayLong();
        }


        public void BreedingAreaBossBattle(string areaId)
        {
            var url = string.Format("http://astrum.amebagames.com/_/event/areaboss/battle?areaId={0}&eventId={1}", areaId, Uri.EscapeDataString(_client.ViewModel.BreedingEventId));
            var result = _client.GetXHR(url);
            AreaBossInfo boss = JsonConvert.DeserializeObject<AreaBossInfo>(result);

            _client.Access("areaboss");
            InfoPrinter.PrintAreaBossInfo(boss, _client.ViewModel);

            var values = new Dictionary<string, object>
            {
                { "areaId", areaId },
                { "eventId", _client.ViewModel.BreedingEventId }
            };
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/event/areaboss/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);
            _client.DelayLong();
        }


    }
}

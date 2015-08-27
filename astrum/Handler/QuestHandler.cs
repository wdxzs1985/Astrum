using Astrum.Http;
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
    public class QuestHandler
    {
        private AstrumClient _client = null;

        public QuestHandler(AstrumClient client)
        {
            _client = client;
        }

        public bool Start()
        {
            _client.Mypage();
            _client.Item();

            return _client.ViewModel.IsQuestEnable;
        }
        
        public void Run()
        {
            _client.Access("stage");

            var viewModel = _client.ViewModel;

            var stage = EnterStage();
            var areaId = stage._id;

            while (viewModel.IsRunning)
            {

                if (stage.isBossStage)
                {
                    AreaBossBattle(areaId);
                    return;
                }
                else if (stage.stageClear && stage.nextStage.isBossStage)
                {
                    stage = ForwardStage(areaId);
                    AreaBossBattle(areaId);
                    return;
                }
                else
                {
                    viewModel.IsFuryRaid = false;
                    viewModel.IsLimitedRaid = false;

                    if (viewModel.IsFuryRaidEnable)
                    {
                        viewModel.IsFuryRaid = true;

                        if (stage.furyraid != null)
                        {
                            if (stage.furyraid.rare == 4)
                            {
                                viewModel.CanFuryRaid = true;
                            }
                        }
                        else if (viewModel.CanFuryRaid)
                        {
                            _client.FuryRaid();
                            viewModel.CanFuryRaid = false;
                            return;
                        }
                        else if (!viewModel.Fever)
                        {
                            if (stage.status.furyraid.find != null)
                            {
                                if (stage.status.furyraid.find.isNew || viewModel.CanFullAttack)
                                {
                                    _client.FuryRaid();
                                    return;
                                }
                            }
                            if (stage.status.furyraid.rescue != null)
                            {
                                if (stage.status.furyraid.rescue.isNew)
                                {
                                    _client.FuryRaid();
                                    return;

                                }
                            }
                        }
                    }

                    if (viewModel.IsLimitedRaidEnable)
                    {
                        var limitedRaidId = stage.status.limitedraid._id;
                        if (limitedRaidId != null)
                        {
                            viewModel.IsLimitedRaid = true;
                            if (viewModel.CanFullAttackForEvent)
                            {
                                _client.LimitedRaid();
                                return;
                            }
                        }
                    }

                    if (stage.status.raid != null && !viewModel.Fever)
                    {

                        viewModel.IsFuryRaid = false;
                        viewModel.IsLimitedRaid = false;
                        viewModel.IsBreedingRaid = false;

                        if (stage.status.raid.find != null)
                        {
                            if (stage.status.raid.find.isNew || viewModel.CanFullAttack)
                            {
                                _client.Raid();
                                return;
                            }
                        }
                        if (stage.status.raid.rescue != null)
                        {
                            if (stage.status.raid.rescue.isNew || viewModel.CanFullAttack)
                            {
                                _client.Raid();
                                return;
                            }
                        }
                    }


                    if (viewModel.IsStaminaEmpty)
                    {
                        bool staminaGreaterThanKeep = viewModel.StaminaValue >= viewModel.KeepStamina;
                        bool staminaGreaterThanExp = viewModel.StaminaValue >= (viewModel.ExpMax - viewModel.ExpValue);
                        bool isBpFull = viewModel.BpValue >= AstrumClient.BP_FULL;
                        bool isFever = viewModel.Fever;

                        if (staminaGreaterThanKeep || staminaGreaterThanExp || isBpFull || isFever)
                        {
                            viewModel.IsStaminaEmpty = false;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (stage.staminaEmpty)
                    {
                        if (stage.items != null && viewModel.ExpMax - viewModel.ExpValue > 100)
                        {
                            var item = stage.items.Find(e => AstrumClient.INSTANT_HALF_STAMINA.Equals(e._id));
                            if (item.stock > viewModel.MinStaminaStock && viewModel.Fever)
                            {
                                _client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_HALF_STAMINA, 1);
                                return;
                            }
                            else
                            {
                                viewModel.IsStaminaEmpty = true;
                                return;
                            }
                        }
                        else
                        {
                            viewModel.IsStaminaEmpty = true;
                            return;
                        }
                    }
                    //forward                   
                    stage = ForwardStage(areaId);
                }

            }
        }


        private StageInfo EnterStage()
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/stage");
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage.initial, _client.ViewModel);
            InfoUpdater.UpdateStageView(stage.initial, _client.ViewModel);
            _client.DelayShort();

            return stage;
        }
        
        private StageInfo ForwardStage(string areaId)
        {
            var values = new Dictionary<string, object>
                {
                   { "areaId", areaId }
                };
            var result = _client.PostXHR("http://astrum.amebagames.com/_/stage", values);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);

            var feverBefore = _client.ViewModel.Fever;
            InfoUpdater.UpdateStageView(stage, _client.ViewModel);
            if (_client.ViewModel.Fever && feverBefore != _client.ViewModel.Fever)
            {
                _client.RaiseNotificationEvent("Fever start", AstrumClient.SECOND * 60);
            }

            _client.DelayShort();
            return stage;
        }
        
        public void AreaBossBattle(string areaId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/areaboss/battle?_id=" + areaId);
            AreaBossInfo boss = JsonConvert.DeserializeObject<AreaBossInfo>(result);
            InfoPrinter.PrintAreaBossInfo(boss, _client.ViewModel);

            _client.Access("areaboss");

            _client.DelayShort();

            var values = new Dictionary<string, object>
            {
                { "_id", areaId }
            };
            var battleResult = _client.PostXHR("http://astrum.amebagames.com/_/areaboss/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            InfoPrinter.PrintBossBattleResult(battleResultInfo, _client.ViewModel);

            _client.DelayLong();
        }
    }
}

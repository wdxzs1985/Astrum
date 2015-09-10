using Astrum.Http;
using Astrum.Json.GuildBattle;
using Astrum.Json.Stage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class GuildBattleHandler
    {
        private AstrumClient _client;

        public GuildBattleHandler(AstrumClient client)
        {
            _client = client;
        }

        public bool Start()
        {
            Schedule schedule = FindSchedule();
            if (schedule != null)
            {
                _client.ViewModel.GuildBattleId = schedule._id;
                GuildBattleInfo battleInfo = GuildBattle(_client.ViewModel.GuildBattleId);

                if (battleInfo.stamp.status)
                {
                    GuildBattleStamp(_client.ViewModel.GuildBattleId);
                    GuildBattleChat();
                    GuildBattleTpInfo();
                }

                return _client.ViewModel.IsGuildBattleEnable;
            }
            else
            {
                _client.ViewModel.GuildBattleId = null;
                _client.ViewModel.History = "没有工会战";
                return false;
            }
        }

        public void Run()
        {
            var battleId = _client.ViewModel.GuildBattleId;

            while (_client.ViewModel.TpValue >= 10 && _client.ViewModel.IsRunning)
            {
                GuildBattleInfo battleInfo = GuildBattle(battleId);
                _client.ViewModel.TpValue = battleInfo.status.tp.value;

                // attack
                var type = "front".Equals(battleInfo.status.position) ? "attack" : "yell";
                var ablility = "front".Equals(battleInfo.status.position) ? "ability_front_attack_default" : "ability_back_yell_default_1";

                GuildBattleCmdInfo cmdInfo = GuildBattleCmd(battleId, type);
                var cmd = cmdInfo.cmd.Find(item => ablility.Equals(item._id));
                if (cmd != null)
                {
                    GuildBattleCmd(battleId, ablility, type);
                }
            }

            TpInfo tpInfo = GuildBattleTpInfo();
            // quest
            TpQuest();

        }



        private Schedule FindSchedule()
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/guildbattle/lobby");
            GuildBattleLobbyInfo lobby = JsonConvert.DeserializeObject<GuildBattleLobbyInfo>(result);

            if (lobby.available && "start".Equals(lobby.status))
            {
                _client.Access("/guildbattle&route=top&value=battle");
                return lobby.schedule.Find(item => "start".Equals(item.status));
            }
            return null;
        }

        private GuildBattleInfo GuildBattle(string battleId)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/guildbattle?_id=" + battleId);
            GuildBattleInfo battleInfo = JsonConvert.DeserializeObject<GuildBattleInfo>(result);


            InfoPrinter.PrintGuildBattleInfo(battleInfo, _client.ViewModel);
            InfoUpdater.UpdateGuildBattleStatus(battleInfo.status, _client.ViewModel);

            _client.DelayShort();
            return battleInfo;
        }

        private void GuildBattleStamp(string battleId)
        {
            var values = new Dictionary<string, object>
                {
                    { "_id", battleId }
                };
            _client.PostXHR("http://astrum.amebagames.com/_/guildbattle/stamp", values);
            _client.DelayShort();
        }

        private void GuildBattleChat()
        {
            var values = new Dictionary<string, object>
            {
                { "stampId", "chat-stamp-004" },
                { "type", "stamp" }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/guild/chat", values);
            _client.DelayShort();
        }

        private GuildBattleCmdInfo GuildBattleCmd(string battleId, string type)
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/guildbattle/cmd?_id=" + battleId + "&type=" + type);
            GuildBattleCmdInfo cmdInfo = JsonConvert.DeserializeObject<GuildBattleCmdInfo>(result);

            //PrintCmdInfo(cmdInfo);
            InfoUpdater.UpdateGuildBattleStatus(cmdInfo.status, _client.ViewModel);

            _client.DelayShort();

            return cmdInfo;
        }


        private void GuildBattleCmd(string battleId, string abilityId, string cmd)
        {
            var values = new Dictionary<string, object>
            {
                { "_id", battleId },
                { "abilityId", abilityId },
                { "cmd", cmd }
            };
            string result = _client.PostXHR("http://astrum.amebagames.com/_/guildbattle/cmd", values);
            var cmdResult = JsonConvert.DeserializeObject<CmdResult>(result);


            if ("success".Equals(cmdResult.commandResult))
            {
                InfoPrinter.PrintGuildBattleCmdResult(cmdResult, _client.ViewModel);
                InfoUpdater.UpdateGuildBattleStatus(cmdResult.battlestate.status, _client.ViewModel);
            }


            _client.DelayLong();
        }


        private TpInfo GuildBattleTpInfo()
        {
            var battleId = _client.ViewModel.GuildBattleId;

            var result = _client.GetXHR("http://astrum.amebagames.com/_/guildbattle/tp?_id=" + Uri.EscapeDataString(battleId));
            TpInfo tpInfo = JsonConvert.DeserializeObject<TpInfo>(result);

            _client.ViewModel.IsTpNormalAvailable = tpInfo.normal.available;
            _client.ViewModel.IsTpChatAvailable = tpInfo.chat.available;
            _client.ViewModel.IsTpRouletteAvailable = tpInfo.roulette.available;

            _client.DelayShort();

            return tpInfo;
        }

        public void GuildBattleTpNormal()
        {

            var battleId = _client.ViewModel.GuildBattleId;
            var values = new Dictionary<string, object>
            {
                { "_id", battleId }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/normal", values);

            _client.ViewModel.History = "回复TP";
            _client.ViewModel.IsTpNormalAvailable = false;
        }

        public void GuildBattleTpChat()
        {
            var battleId = _client.ViewModel.GuildBattleId;
            var values = new Dictionary<string, object>
            {
                { "_id", battleId }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/chat", values);

            _client.ViewModel.History = "回复TP";
            _client.ViewModel.IsTpChatAvailable = false;
        }

        public void GuildBattleTpRoulette()
        {
            var battleId = _client.ViewModel.GuildBattleId;
            var result = _client.GetXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette?_id=" + Uri.EscapeDataString(battleId));
            Roulette roulette = JsonConvert.DeserializeObject<Roulette>(result);
            
            int targetPosition = roulette.order.IndexOf(80);
            int position = roulette.initialPosition - targetPosition;
            
            var values = new Dictionary<string, object>
            {
                { "_id", battleId },
                { "position", position }
            };
            _client.PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette", values);

            _client.ViewModel.History = "回复TP";
            _client.ViewModel.IsTpRouletteAvailable = false;
        }

        private void TpQuest()
        {
            _client.Access("stage");

            var stage = EnterTpStage();

            while (_client.ViewModel.IsRunning)
            {
                if (stage.status.tp.value > 80)
                {
                    return;
                }
                if (stage.staminaEmpty)
                {
                    return;
                }

                //forward
                stage = ForwardTpStage();
            }
        }

        private StageInfo EnterTpStage()
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/stage/tp");
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);
            InfoUpdater.UpdateStageView(stage, _client.ViewModel);

            _client.DelayShort();

            return stage;
        }

        private StageInfo ForwardTpStage()
        {
            var values = new Dictionary<string, object>
                {
                   { "areaId", "recovery_tp" }
                };
            var result = _client.PostXHR("http://astrum.amebagames.com/_/stage/tp", values);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);
            InfoUpdater.UpdateStageView(stage, _client.ViewModel);
            _client.DelayShort();

            return stage;
        }
    }
}

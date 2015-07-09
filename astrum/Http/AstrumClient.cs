using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net;
using System.IO;
using Newtonsoft.Json;
using Astrum.Json.Mypage;
using Astrum.Json.Stage;
using Astrum.Json.Raid;
using Astrum.Json.Event;

namespace Astrum.Http
{

    public class AstrumClient : HttpClient
    {
        public const string XHR = "XMLHttpRequest";
        public const string PUT = "PUT";

        public const int SECOND = 1000;
        public const int MINUTE = 60 * SECOND;

        public const int DELAY_LONG = SECOND;
        public const int DELAY_SHORT = SECOND;
        public const int NO_DELAY = 0;

        public const int MIN_STAMINA_STOCK = 9999;

        public AstrumClient()
        {
            ViewModel = new ViewModel();

            ViewModel.IsQuestEnable = true;
            ViewModel.IsRaidEnable = false;
            ViewModel.IsGuildBattleEnable = false;
            ViewModel.IsUnlimitStage = false;
        }

        public ViewModel ViewModel { get; set; }

        private string xGroup = "a";
        private string xRtoken = "undefined";
        private string xUtoken = "undefined";
        private string xVersion = "undefined";

        private Random seed = new Random();

        public void Delay(int time)
        {
            if (time > 0)
            {
                int randomTime = time + seed.Next(time);

                for (var i = 0; i < randomTime; i += 100)
                {
                    Thread.Sleep(100);
                    if (i % 1000 == 0)
                    {
                        Console.WriteLine("wait {0} second", (randomTime - i) / 1000);
                    }
                }
            }
        }

        protected string GetXHR(string url)
        {
            Console.WriteLine("[GET ] " + url);

            var request = this.CreateRequest(url);
            ConfigXHRHeaders(request);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result = ResponseToString(response);
            //Console.WriteLine(result);

            RefreshToken(response);

            response.Close();
            return result;
        }

        protected string PostXHR(string url, Dictionary<string, string> values)
        {
            Console.WriteLine("[POST] " + url);

            var request = this.CreateRequest(url);
            request.Headers.Add("X-HTTP-Method-Override", PUT);
            ConfigXHRHeaders(request);

            PostJson(request, values);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result = ResponseToString(response);
            //Console.WriteLine(result);

            RefreshToken(response);

            response.Close();

            return result;
        }

        protected void ConfigXHRHeaders(HttpWebRequest request)
        {
            request.Accept = "application/json";
            request.Referer = "http://astrum.amebagames.com/";
            request.ContentType = "application/json";

            request.Headers.Add("X-Requested-With", XHR);
            request.Headers.Add("X-Group", xGroup);
            request.Headers.Add("X-Rtoken", xRtoken);
            request.Headers.Add("X-Utoken", xUtoken);
            request.Headers.Add("X-Version", xVersion);
        }

        protected void RefreshToken(HttpWebResponse response)
        {
            if (response.Headers.Get("X-Group") != null)
            {
                xGroup = response.Headers.Get("X-Group");
                //Console.WriteLine("  X-Group: {0}", xGroup);
            }
            if (response.Headers.Get("X-Rtoken") != null)
            {
                xRtoken = response.Headers.Get("X-Rtoken");
                //Console.WriteLine(" X-Rtoken: {0}", xRtoken);
            }
            if (response.Headers.Get("X-Utoken") != null)
            {
                xUtoken = response.Headers.Get("X-Utoken");
                //Console.WriteLine(" X-Utoken: {0}", xUtoken);
            }
            if (response.Headers.Get("X-Version") != null)
            {
                xVersion = response.Headers.Get("X-Version");
                //Console.WriteLine("X-Version: {0}", xVersion);
            }
        }

        public bool Login(string username, string password)
        {
            var values = new Dictionary<string, string>
            {
               { "username", username },
               { "password", password }
            };

            string html = Post("https://login.user.ameba.jp/web/login", values);

            Get("http://astrum.amebagames.com/login");

            return true;
        }

        public void Token()
        {
            this.GetXHR("http://astrum.amebagames.com/_/token");
        }

        public void Access(string path)
        {
            var p = Uri.EscapeDataString(path);
            this.GetXHR("http://astrum.amebagames.com/_/access?p=" + path);
        }

        public void Mypage()
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/mypage");
            var mypage = JsonConvert.DeserializeObject<MypageInfo>(responseString);

            PrintMypage(mypage);
            UpdateMypageView(mypage);

            Access("mypage");
        }


        public void Quest()
        {
            Access("stage");

            while (ViewModel.IsRunning)
            {
                var areaId = "chapter1-1";
                var stage = EnterStage(areaId);

                while (ViewModel.IsRunning)
                {
                    if (stage.isBossStage)
                    {
                        AreaBossBattle(areaId);
                        break;
                    }
                    else if ((stage.stageClear && stage.nextStage.isBossStage) && !ViewModel.IsUnlimitStage)
                    {
                        stage = ForwardStage(areaId);
                        AreaBossBattle(areaId);
                        break;
                    }
                    else
                    {
                        //raid
                        if (stage.status.raid != null)
                        {
                            bool canFull = stage.status.bp.value >= 3;

                            if (stage.status.raid.find != null && (stage.status.raid.find.isNew || canFull))
                            {
                                var loop = true;
                                while (loop)
                                {
                                    loop = RaidBattle(stage.status.raid.find._id);
                                }
                            }
                            if (stage.status.raid.rescue != null && (stage.status.raid.rescue.isNew || canFull))
                            {
                                var loop = true;
                                while (loop)
                                {
                                    loop = RaidBattle(stage.status.raid.rescue._id);
                                }
                            }
                        }

                        if (stage.staminaEmpty)
                        {
                            if (stage.items != null)
                            {
                                var item = stage.items.Find(e => "instant-half_stamina_potion".Equals(e._id));
                                if (item.stock > MIN_STAMINA_STOCK)
                                {
                                    UseItem(item, "stamina");
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        //forward
                        stage = ForwardStage(areaId);
                    }
                }
            }
        }

        private StageInfo EnterStage(string areaId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/stage?areaId=" + areaId);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            PrintStageInfo(stage);
            UpdateStageView(stage);
            Delay(DELAY_SHORT);

            return stage;
        }

        private StageInfo ForwardStage(string areaId)
        {
            var values = new Dictionary<string, string>
                {
                   { "areaId", areaId }
                };
            var result = PostXHR("http://astrum.amebagames.com/_/stage", values);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            PrintStageInfo(stage);
            UpdateStageView(stage);
            Delay(DELAY_SHORT);

            return stage;
        }

        public void UseItem(Item item, string type)
        {
            GetXHR("http://astrum.amebagames.com/_/item/common?type=" + type);

            var values = new Dictionary<string, string>
            {
                { "itemId", item._id },
                { "value", "1" }
            };
            PostXHR("http://astrum.amebagames.com/_/item/common", values);
            Delay(DELAY_SHORT);
        }

        public void AreaBossBattle(string areaId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/areaboss/battle?_id=" + areaId);
            AreaBossInfo boss = JsonConvert.DeserializeObject<AreaBossInfo>(result);
            PrintAreaBossInfo(boss);

            Access("areaboss");

            Delay(DELAY_SHORT);

            var values = new Dictionary<string, string>
            {
                { "_id", areaId }
            };
            var battleResult = PostXHR("http://astrum.amebagames.com/_/areaboss/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            PrintBossBattleResult(battleResultInfo);
            Delay(DELAY_LONG);
        }

        public void Raid()
        {
            GetXHR("http://astrum.amebagames.com/_/event/raid");
            Access("event_raid");
            Delay(DELAY_SHORT);

            var result = GetXHR("http://astrum.amebagames.com/_/raid");
            var raidInfo = JsonConvert.DeserializeObject<RaidInfo>(result);
            Delay(DELAY_SHORT);

            if (raidInfo.find != null)
            {
                var battleInfo = raidInfo.find;
                var loop = true;
                while (loop)
                {
                    loop = RaidBattle(battleInfo._id);
                }
            }

            if (raidInfo.rescue != null)
            {
                foreach (var battleInfo in raidInfo.rescue.list)
                {
                    RaidBattle(battleInfo._id);
                }
            }
        }

        public bool RaidBattle(string raidId)
        {
            var battleInfo = BattleInfo(raidId);

            if (battleInfo.isWin || battleInfo.isLose)
            {
                return false;
            }

            if (battleInfo.isNew)
            {
                RaidBattleAttack(battleInfo._id, "first");
                return true;
            }

            if (battleInfo.rescue.use)
            {
                RaidBattleRescue(battleInfo._id);
            }

            if (battleInfo.bpValue >= 3)
            {
                RaidBattleAttack(battleInfo._id, "full");
                return true;
            }
            return false;
        }

        private RaidBattleInfo BattleInfo(string raidId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/raid/battle?_id=" + raidId);
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            PrintRaidBattleInfo(battleInfo);
            Delay(DELAY_SHORT);

            return battleInfo;
        }

        private void RaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = PostXHR("http://astrum.amebagames.com/_/raid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            PrintBossBattleResult(battleResultInfo);
            Delay(DELAY_LONG);

        }

        private void RaidBattleRescue(string raidId)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId }
            };
            PostXHR("http://astrum.amebagames.com/_/raid/battlerescue", values);
            Delay(DELAY_SHORT);

        }

        public void GuildBattle()
        {
            GuildBattleLobbyInfo lobby = GuildBattleLobby();
            Schedule schedule = FindSchedule(lobby);

            if (schedule != null)
            {
                GuildBattleInfo battleInfo = GuildBattle(schedule._id);

                if (battleInfo.stamp.status)
                {
                    GetXHR("http://astrum.amebagames.com/_/guildbattle/stamp");
                }


                while (true)
                {

                }
            }
        }

        private GuildBattleLobbyInfo GuildBattleLobby()
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/lobby");
            return JsonConvert.DeserializeObject<GuildBattleLobbyInfo>(result);
        }

        private Schedule FindSchedule(GuildBattleLobbyInfo lobby)
        {
            if (lobby.available && "start".Equals(lobby.status))
            {
                Access("p=/guildbattle&route=top&value=battle");
                return lobby.schedule.Find(item => "start".Equals(item.status));
            }
            return null;
        }

        private GuildBattleInfo GuildBattle(string battleId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle?_id=" + battleId);
            GuildBattleInfo battleInfo = JsonConvert.DeserializeObject<GuildBattleInfo>(result);
            return battleInfo;
        }

        private void PrintMypage(MypageInfo mypage)
        {
            Console.WriteLine("   Name: {0} (L{1})", mypage.status.name, mypage.status.level);
            Console.WriteLine("  Total: {0} (ATK: {1}, DF: {2}, MAT: {3}, MDF: {4})", mypage.total, mypage.status.atk, mypage.status.df, mypage.status.mat, mypage.status.mdf);
            Console.WriteLine("Stamina: {0} / {1}", mypage.status.stamina_value, mypage.status.stamina_max);
            Console.WriteLine("    EXP: {0} / {1}", mypage.status.exp_value, mypage.status.exp_max);
            Console.WriteLine("     BP: {0} / {1}", mypage.status.bp_value, mypage.status.bp_max);
            Console.WriteLine("     TP: {0} / {1}", mypage.status.tp_value, mypage.status.tp_max);
            Console.WriteLine("  Guild: {0}, Rank: {1}", mypage.guild.name, mypage.guild.rank);
            Console.WriteLine("  Quest: {0}", mypage.link.quest._id);
        }

        private void PrintStageInfo(StageInfo stage)
        {
            Console.WriteLine("    Name: {0}[{1}] ({2})", stage.name, stage.stage, stage.isBossStage ? "BOSS" : stage.progress + "%");

            if (stage.status != null)
            {
                Console.WriteLine(" Stamina: {0} / {1}", stage.status.stamina.value, stage.status.stamina.max);
                Console.WriteLine("     EXP: {0} / {1}", stage.status.exp.value, stage.status.exp.max);
                Console.WriteLine("      BP: {0} / {1}", stage.status.bp.value, stage.status.bp.max);
                Console.WriteLine("      TP: {0} / {1}", stage.status.tp.value, stage.status.tp.max);
            }
        }

        private void PrintAreaBossInfo(AreaBossInfo boss)
        {
            Console.WriteLine("   Name: {0}", boss.name);
            Console.WriteLine("   Area: {0}", boss.areaName);
            Console.WriteLine("     HP: {0} / {1}", boss.hp - boss.totalDamage, boss.hp);
        }

        private void PrintRaidBattleInfo(RaidBattleInfo battleInfo)
        {
            Console.WriteLine("   Name: {0} (L{1})", battleInfo.name, battleInfo.level);
            Console.WriteLine("   Rare: {0}", battleInfo.rare);
            Console.WriteLine("     HP: {0} / {1}", battleInfo.hp - battleInfo.totalDamage, battleInfo.hp);
        }

        private void PrintBossBattleResult(BossBattleResultInfo resultInfo)
        {
            if (resultInfo.result != null)
            {
                Console.WriteLine("  Result: {0}", resultInfo.result.resultType);
                Console.WriteLine("  BossHP: {0} / {1}", resultInfo.result.afterBoss.hp, resultInfo.result.afterBoss.maxHp);
            }
        }


        private void UpdateMypageView(MypageInfo mypage)
        {
            ViewModel.Name = mypage.status.name;
            ViewModel.Level = mypage.status.level;

            ViewModel.StaminaValue = mypage.status.stamina_value;
            ViewModel.StaminaMax = mypage.status.stamina_max;

            ViewModel.ExpValue = mypage.status.exp_value;
            ViewModel.ExpMin = mypage.status.exp_min;
            ViewModel.ExpMax = mypage.status.exp_max;

            ViewModel.BpValue = mypage.status.bp_value;
            ViewModel.BpMax = mypage.status.bp_max;

            ViewModel.TpValue = mypage.status.tp_value;
            ViewModel.TpMax = mypage.status.tp_max;
        }

        private void UpdateStageView(StageInfo stage)
        {
            if (stage.status != null)
            {
                ViewModel.Level = stage.status.level;

                ViewModel.StaminaValue = stage.status.stamina.value;
                ViewModel.StaminaMax = stage.status.stamina.max;

                ViewModel.ExpValue = stage.status.exp.value;
                ViewModel.ExpMin = stage.status.exp.min;
                ViewModel.ExpMax = stage.status.exp.max;

                ViewModel.BpValue = stage.status.bp.value;
                ViewModel.BpMax = stage.status.bp.max;

                ViewModel.TpValue = stage.status.tp.value;
                ViewModel.TpMax = stage.status.tp.max;
            }
        }

    }
}

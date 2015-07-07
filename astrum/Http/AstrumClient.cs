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

namespace Astrum
{

    public class AstrumClient : HttpClient
    {
        public const string XHR = "XMLHttpRequest";
        public const string PUT = "PUT";

        public const int DELAY_LONG = 2000;
        public const int DELAY_SHORT = 500;

        public AstrumClient()
        {
        }

        public string Username { get; set; }
        public string Password { get; set; }

        private string xGroup = "a";
        private string xRtoken = "undefined";
        private string xUtoken = "undefined";
        private string xVersion = "undefined";

        private Mypage __mypage;

        private Random seed = new Random();

        public void delay(int time)
        {
            Thread.Sleep(time + seed.Next(time));
        }

        protected string GetXHR(string url)
        {
            Console.WriteLine("[GET ] " + url);

            var request = this.CreateRequest(url);
            ConfigXHRHeaders(request);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result = ResponseToString(response);
            Console.WriteLine(result);

            RefreshToken(response);

            response.Close();

            return result;
        }

        protected string PostXHR(string url, Dictionary<string,string> values)
        {
            Console.WriteLine("[POST] " + url);

            var request = this.CreateRequest(url);
            request.Headers.Add("X-HTTP-Method-Override", PUT);
            ConfigXHRHeaders(request);

            PostJson(request, values);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result = ResponseToString(response);
            Console.WriteLine(result);

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
                Console.WriteLine("  X-Group: {0}", xGroup);
            }
            if (response.Headers.Get("X-Rtoken") != null)
            {
                xRtoken = response.Headers.Get("X-Rtoken");
                Console.WriteLine(" X-Rtoken: {0}", xRtoken);
            }
            if (response.Headers.Get("X-Utoken") != null)
            {
                xUtoken = response.Headers.Get("X-Utoken");
                Console.WriteLine(" X-Utoken: {0}", xUtoken);
            }
            if (response.Headers.Get("X-Version") != null)
            {
                xVersion = response.Headers.Get("X-Version");
                Console.WriteLine("X-Version: {0}", xVersion);
            }
        }

        public bool Login()
        {
            var values = new Dictionary<string, string>
            {
               { "username", Username },
               { "password",Password }
            };

            string html = Post("https://login.user.ameba.jp/web/login", values);

            Get("http://astrum.amebagames.com/login");

            return true;
        }

        public void Token()
        {
           this.GetXHR("http://astrum.amebagames.com/_/token");
        }

        public void Mypage()
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/mypage");
            __mypage = JsonConvert.DeserializeObject<Mypage>(responseString);
            Console.WriteLine("   Name: {0} (L{1})", __mypage.status.name, __mypage.status.level);
            Console.WriteLine("  Total: {0} (ATK: {1}, DF: {2}, MAT: {3}, MDF: {4})", __mypage.total, __mypage.status.atk, __mypage.status.df, __mypage.status.mat, __mypage.status.mdf);
            Console.WriteLine("Stamina: {0} / {1}", __mypage.status.stamina_value, __mypage.status.stamina_max);
            Console.WriteLine("    EXP: {0} / {1}", __mypage.status.exp_value, __mypage.status.exp_max);
            Console.WriteLine("     BP: {0} / {1}", __mypage.status.bp_value, __mypage.status.bp_max);
            Console.WriteLine("     TP: {0} / {1}", __mypage.status.tp_value, __mypage.status.tp_max);
            Console.WriteLine("  Guild: {0}, Rank: {1}", __mypage.guild.name, __mypage.guild.rank);
            Console.WriteLine("  Quest: {0}", __mypage.link.quest._id);
            //Console.WriteLine("   Raid: isNew: {0}, canCombo: {1}", __mypage.link.raid.isNew, __mypage.link.raid.canCombo);

        }

        public void Quest()
        {
            var result = GetXHR("http://astrum.amebagames.com/_/stage?areaId=" + __mypage.link.quest._id);
            var stage = JsonConvert.DeserializeObject<Stage>(result);
            printStageInfo(stage);
            delay(DELAY_SHORT);

            while (true)
            {
                if (stage.status.raid != null)
                {
                    if (stage.status.raid.find != null && stage.status.raid.find.isNew)
                    {
                        RaidBattle(stage.status.raid.find._id);
                    }
                    if (stage.status.raid.rescue != null && stage.status.raid.rescue.isNew)
                    {
                        RaidBattle(stage.status.raid.rescue._id);
                    }
                }


                var areaId = stage._id;
                var values = new Dictionary<string, string>
                {
                   { "areaId", areaId }
                };
                result = PostXHR("http://astrum.amebagames.com/_/stage", values);
                stage = JsonConvert.DeserializeObject<Stage>(result);
                printStageInfo(stage);

                if (stage.staminaEmpty)
                {
                    break;
                }

                if (stage.nextStage != null)
                {
                    stage = stage.nextStage;
                }

                delay(DELAY_LONG);
            }
        }

        private void printStageInfo(Stage stage)
        {
            Console.WriteLine("   Name: {0} ({1}%)", stage.name, stage.progress);
            Console.WriteLine("Stamina: {0} / {1}", stage.status.stamina.value, stage.status.stamina.max);
            Console.WriteLine("    EXP: {0} / {1}", stage.status.exp.value, stage.status.exp.max);
            Console.WriteLine("     BP: {0} / {1}", stage.status.bp.value, stage.status.bp.max);
            Console.WriteLine("     TP: {0} / {1}", stage.status.tp.value, stage.status.tp.max);

            //Raid
        }

        public void Raid()
        {
            var result = GetXHR("http://astrum.amebagames.com/_/raid");
            var raidInfo = JsonConvert.DeserializeObject<RaidInfo>(result);

            if (raidInfo.find != null)
            {
                var battleInfo = raidInfo.find;
                RaidBattle(battleInfo._id);
            }

            if (raidInfo.rescue != null)
            {
                foreach (var battleInfo in raidInfo.rescue.list)
                {
                    RaidBattle(battleInfo._id);
                }
            }
        }

        public void RaidBattle(string raidId)
        {
            while (true)
            {
                var result = GetXHR("http://astrum.amebagames.com/_/raid/battle?_id=" + raidId);
                var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

                printRaidBattleInfo(battleInfo);
                delay(DELAY_SHORT);

                if (battleInfo.isNew)
                {
                    var values = new Dictionary<string, string>
                    {
                       { "_id", raidId },
                       { "attackType", "first" }
                    };
                    //first
                    var battleResult = PostXHR("http://astrum.amebagames.com/_/raid/battle", values);
                    var battleResultInfo = JsonConvert.DeserializeObject<RaidBattleResultInfo>(battleResult);

                    printRaidBattleResult(battleResultInfo);

                    delay(DELAY_LONG);
                }
                else if ("find".Equals(battleInfo.type) && !battleInfo.rescue.use) {
                    RaidRescue(battleInfo._id);
                }
                else
                {
                    break;
                }
            }
        }

        public void RaidRescue(string raidId)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId }
            };
            PostXHR("http://astrum.amebagames.com/_/raid/battlerescue", values);
        }

        private void printRaidBattleInfo(RaidBattleInfo battleInfo)
        {
            Console.WriteLine("   Name: {0} ({1}%)", battleInfo.name, battleInfo.level);
            Console.WriteLine("   Rare: {0}", battleInfo.rare);
            Console.WriteLine("     HP: {0} (-{1})", battleInfo.hp, battleInfo.totalDamage);
        }

        private void printRaidBattleResult(RaidBattleResultInfo battleResultInfo)
        {
            if (battleResultInfo.result != null)
            {
                Console.WriteLine("  Result: {0}", battleResultInfo.result.resultType);
                Console.WriteLine("  BossHP: {0} / {1}", battleResultInfo.result.afterBoss.hp, battleResultInfo.result.afterBoss.maxHp);
            }
        }

    }
}

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
using Astrum.Json.GuildBattle;
using Astrum.Json.Item;
using Astrum.Json.Gift;

namespace Astrum.Http
{

    public class AstrumClient : HttpClient
    {
        public const string XHR = "XMLHttpRequest";
        public const string PUT = "PUT";

        public const int INTERTAL = 100;
        public const int SECOND = 10 * INTERTAL;
        public const int MINUTE = 60 * SECOND;

        public const int DELAY_LONG = SECOND;
        public const int DELAY_SHORT = SECOND;
        public const int NO_DELAY = 0;

        public const string ITEM_STAMINA = "stamina";
        public const string ITEM_BP = "bp";

        public const string INSTANT_HALF_STAMINA = "instant-half_stamina_potion";
        public const string INSTANT_STAMINA = "instant-stamina_potion";
        public const string INSTANT_BP = "instant-bp_ether";
        public const string INSTANT_MINI_BP = "instant-mini_bp_ether";

        public const string FULL = "full";
        public const string NORMAL = "normal";
        public const int BP_FULL = 3;
        public const int BP_NORMAL = 1;

        public const string FIRST = "first";
        public const string FIND = "find";
        public const string RESCUE = "rescue";


        public const int MIN_STOCK = 9999;

        public const int EASY_BOSS_HP = 2000000;

        public AstrumClient()
        {
            ViewModel = new ViewModel();

            ViewModel.IsQuestEnable = true;
            ViewModel.IsGuildBattleEnable = false;

            ViewModel.MinStaminaStock = MIN_STOCK;
            ViewModel.MinBpStock = MIN_STOCK;
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

                for (var i = 0; i < randomTime; i += INTERTAL)
                {
                    Thread.Sleep(INTERTAL);
                }
            }
        }

        public void CountDown(int countDown)
        {
            for (var i = 0; i < countDown; i += INTERTAL)
            {
                Thread.Sleep(INTERTAL);
                if (ViewModel.IsRunning)
                {
                    if (i % SECOND == 0)
                    {
                        var message = String.Format("少女休息中。。。 {0} 秒", (countDown - i) / SECOND);
                        ViewModel.History = message;
                    }
                }
                else
                {
                    var message = "少女休息中。。。 ";
                    ViewModel.History = message;
                    break;
                }
            }
        }

        protected string GetXHR(string url)
        {
            Console.WriteLine("[GET ] " + url);

            var request = this.CreateRequest(url);
            ConfigXHRHeaders(request);

            HttpWebResponse response = null;
            string result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                result = ResponseToString(response);
                //Console.WriteLine(result);

                RefreshToken(response);
            }
            catch (Exception ex)
            {
                ViewModel.History = ex.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

        protected string PostXHR(string url, Dictionary<string, string> values)
        {
            Console.WriteLine("[POST] " + url);

            var request = this.CreateRequest(url);
            request.Headers.Add("X-HTTP-Method-Override", PUT);
            ConfigXHRHeaders(request);

            PostJson(request, values);

            HttpWebResponse response = null;
            string result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                result = ResponseToString(response);
                //Console.WriteLine(result);

                RefreshToken(response);
            }
            catch (Exception ex)
            {
                ViewModel.History = ex.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
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

            clearCookie();

            Post("https://login.user.ameba.jp/web/login", values);
            Get("http://astrum.amebagames.com/login");
            this.GetXHR("http://astrum.amebagames.com/_/token");

            return true;
        }

        public void Access(string path)
        {
            var p = Uri.EscapeDataString(path);
            this.GetXHR("http://astrum.amebagames.com/_/access?p=" + path);
        }

        public void OnStart()
        {
            this.Mypage();
            this.Gift();
            this.Item();
            this.EventStatus();
        }

        public void Mypage()
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/mypage");
            var mypage = JsonConvert.DeserializeObject<MypageInfo>(responseString);

            PrintMypage(mypage);
            UpdateMypageView(mypage);

            Access("mypage");
            Delay(DELAY_SHORT);

            if (mypage.loginBonus != null)
            {
                if (mypage.loginBonus.basic)
                {
                    LoginBonusBasic();
                }
                if (mypage.loginBonus.@event)
                {
                    LoginBonusEvent();
                }
                if (mypage.loginBonus.longLogin)
                {
                    LoginBonusLongLogin();
                }
            }

        }

        private void LoginBonusBasic()
        {
            GetXHR("http://astrum.amebagames.com/_/loginbonus");
            Delay(DELAY_SHORT);
        }

        private void LoginBonusEvent()
        {
            GetXHR("http://astrum.amebagames.com/_/loginbonus/event");
            Delay(DELAY_SHORT);
        }

        private void LoginBonusLongLogin()
        {
            GetXHR("http://astrum.amebagames.com/_/loginbonus/long");
            Delay(DELAY_SHORT);
        }

        public void Gift()
        {
            while (true)
            {
                var result = GetXHR("http://astrum.amebagames.com/_/gift?page=1&size=10&type=all&limited=1");
                GiftInfo giftInfo = JsonConvert.DeserializeObject<GiftInfo>(result);

                Access("gift");
                Delay(DELAY_SHORT);

                if (giftInfo.total > 0)
                {
                    var values = new Dictionary<string, string>
                    {
                       { "auto", "1" },
                       { "limited", "1" },
                       { "type", "all" }
                    };
                    PostXHR("http://astrum.amebagames.com/_/gift", values);
                    Delay(DELAY_SHORT);
                }
                else
                {
                    return;
                }
            }
        }

        public void Item()
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/item");
            var itemList = JsonConvert.DeserializeObject<ItemList>(responseString);

            foreach (var item in itemList.list)
            {
                UpdateItemStock(item);
            }

            Access("item");
            Delay(DELAY_SHORT);

        }

        public void EventStatus()
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/event/status");
            var eventStatus = JsonConvert.DeserializeObject<EventStatus>(responseString);

            foreach (var @event in eventStatus.list)
            {
                if (@event.status)
                {
                    if ("furyraid".Equals(@event.type))
                    {
                        ViewModel.IsFuryRaidEnable = true;
                        ViewModel.FuryRaidEventId = @event._id;

                        FuryRaidFever();
                    }
                    else if ("limitedraid".Equals(@event.type))
                    {
                        ViewModel.IsLimitedRaidEnable = true;
                        ViewModel.LimitedRaidEventId = @event._id;
                        
                        LimitedRaidInfo();

                    }
                }
            }
        }

        public void Quest()
        {

            Access("stage");

            //var areaId = "chapter1-1";
            var stage = EnterStage();
            var areaId = stage._id;

            while (ViewModel.IsRunning)
            {

                if (stage.isBossStage)
                {
                    AreaBossBattle(areaId);
                    break;
                }
                else if (stage.stageClear && stage.nextStage.isBossStage)
                {
                    stage = ForwardStage(areaId);
                    AreaBossBattle(areaId);
                    break;
                }
                else
                {
                    ViewModel.IsFuryRaid = false;
                    ViewModel.IsLimitedRaid = false;

                    //furyraid
                    if (ViewModel.IsFuryRaidEnable)
                    {
                        ViewModel.IsFuryRaid = true;

                        if (stage.status.furyraid.find != null)
                        {
                            if (stage.status.furyraid.find.isNew || ViewModel.CanFullAttackForEvent)
                            {
                                FuryRaid();
                            }
                        }
                        if (stage.status.furyraid.rescue != null)
                        {
                            if (stage.status.furyraid.rescue.isNew || ViewModel.CanFullAttack)
                            {
                                FuryRaid();
                            }
                        }
                    }

                    if (ViewModel.IsLimitedRaidEnable)
                    {
                        ViewModel.IsLimitedRaid = true;

                        var limitedRaidId = stage.status.limitedraid._id;
                        if (limitedRaidId != null)
                        {
                            if(ViewModel.CanFullAttackForEvent)
                            {
                                LimitedRaid();
                            }
                        }
                    }

                    if (stage.status.raid != null)
                    {
                        ViewModel.IsFuryRaid = false;
                        ViewModel.IsLimitedRaid = false;

                        if (stage.status.raid.find != null )
                        {
                            if (stage.status.raid.find.isNew || ViewModel.CanFullAttack)
                            {
                                Raid();
                            }
                        }
                        if (stage.status.raid.rescue != null)
                        {
                            if (stage.status.raid.rescue.isNew || ViewModel.CanFullAttack)
                            {
                                Raid();
                            }
                        }
                    }


                    if (ViewModel.IsStaminaEmpty)
                    {
                        bool staminaGreaterThanKeep = ViewModel.StaminaValue >= ViewModel.MaxKeepStamina;
                        bool staminaGreaterThanExp = ViewModel.StaminaValue >= (ViewModel.ExpMax - ViewModel.ExpValue);
                        bool isBpFull = ViewModel.BpValue >= BP_FULL;
                        bool isFever = ViewModel.Fever;

                        if (staminaGreaterThanKeep || staminaGreaterThanExp || isBpFull || isFever)
                        {
                            ViewModel.IsStaminaEmpty = false;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (stage.staminaEmpty)
                    {
                        if (stage.items != null && ViewModel.ExpMax - ViewModel.ExpValue > 100)
                        {
                            var item = stage.items.Find(e => INSTANT_HALF_STAMINA.Equals(e._id));
                            if (item.stock > ViewModel.MinStaminaStock && ViewModel.Fever)
                            {
                                UseItem(ITEM_STAMINA, INSTANT_HALF_STAMINA, 1);
                            }
                            else
                            {
                                ViewModel.IsStaminaEmpty = true;
                                return;
                            }
                        }
                        else
                        {
                            ViewModel.IsStaminaEmpty = true;
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
            var result = GetXHR("http://astrum.amebagames.com/_/stage");
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

        public void UseItem(string type, string itemId, int value)
        {
            var responseString = GetXHR("http://astrum.amebagames.com/_/item/common?type=" + type);
            var itemList = JsonConvert.DeserializeObject<ItemList>(responseString);

            var item = itemList.list.Find(e => itemId.Equals(e._id));

            if (item.stock >= value)
            {
                var values = new Dictionary<string, string>
                {
                    { "itemId", item._id },
                    { "value", value.ToString() }
                };
                string result = PostXHR("http://astrum.amebagames.com/_/item/common", values);
                var useItemResult = JsonConvert.DeserializeObject<UseItemResult>(result);

                UpdateItemStock(useItemResult);

                Delay(DELAY_SHORT);
            }
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
                var loop = battleInfo.isNew || ViewModel.CanFullAttack;
                while (loop)
                {
                    loop = RaidBattle(battleInfo._id);
                }
            }

            if (raidInfo.rescue != null)
            {
                foreach (var battleInfo in raidInfo.rescue.list)
                {
                    var loop = battleInfo.isNew;
                    while (loop)
                    {
                        loop = RaidBattle(battleInfo._id);
                    }
                }
            }
        }

        public bool RaidBattle(string raidId)
        {
            var battleInfo = BattleInfo(raidId);
            if (battleInfo.isPlaying)
            {
                if (battleInfo.isNew)
                {
                    RaidBattleAttack(battleInfo._id, FIRST);
                    return true;
                }

                if (battleInfo.rescue.use)
                {
                    RaidBattleRescue(battleInfo._id);
                    return true;
                }

                if (ViewModel.CanAttack)
                {
                    var hp = battleInfo.hp - battleInfo.totalDamage;
                    var attackType = hp > EASY_BOSS_HP ? FULL : NORMAL;
                    var needBp = hp > EASY_BOSS_HP ? BP_FULL : BP_NORMAL;

                    if (ViewModel.BpValue >= needBp)
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
            var result = GetXHR("http://astrum.amebagames.com/_/raid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            UpdateBpAfterRaidBattle(battleInfo);

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

        private void RaidBattleResult(string raidId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/raid/battleresult?_id=" + Uri.EscapeDataString(raidId));
            RaidBattleInfo battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            Delay(DELAY_SHORT);
            GetXHR("http://astrum.amebagames.com/_/raid/summary");

            Delay(DELAY_SHORT);
        }


        public void FuryRaid()
        {
            FuryRaidFever();
            FuryRaidInfo raidInfo = FuryRaidInfo();

            if (raidInfo.find != null)
            {
                foreach (var battleInfo in raidInfo.find.list)
                {
                    var loop = battleInfo.isNew || (ViewModel.CanFullAttackForEvent);
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
                    var loop = battleInfo.isNew;
                    while (loop)
                    {
                        loop = FuryRaidBattle(battleInfo._id);
                    }
                }
            }
        }

        public void FuryRaidFever()
        {
            Access("furyraid");

            var eventId = ViewModel.FuryRaidEventId;
            string result = GetXHR("http://astrum.amebagames.com/_/event/furyraid?_id=" + Uri.EscapeDataString(eventId));
            FuryRaidInfo raidInfo = JsonConvert.DeserializeObject<FuryRaidInfo>(result);

            ViewModel.Fever = raidInfo.fever.progress == 100;

            Delay(DELAY_SHORT);
        }

        public FuryRaidInfo FuryRaidInfo()
        {
            var eventId = ViewModel.FuryRaidEventId;
            string result = GetXHR("http://astrum.amebagames.com/_/event/furyraid/bosses?_id=" + Uri.EscapeDataString(eventId));
            FuryRaidInfo raidInfo = JsonConvert.DeserializeObject<FuryRaidInfo>(result);

            Delay(DELAY_SHORT);
            return raidInfo;
        }

        public bool FuryRaidBattle(string raidId)
        {
            var battleInfo = FuryBattleInfo(raidId);

            if (battleInfo.isPlaying)
            {
                if (battleInfo.isNew)
                {
                    var attackType = FIRST;
                    if (RESCUE.Equals(battleInfo.type))
                    {
                        attackType = RESCUE;
                    }

                    FuryRaidBattleAttack(battleInfo._id, attackType);
                    return true;
                }

                if (battleInfo.rescue.use)
                {
                    FuryRaidBattleRescue(battleInfo._id);
                }

                if (FIND.Equals(battleInfo.type))
                {
                    var hp = battleInfo.hp - battleInfo.totalDamage;

                    var attackType = hp > EASY_BOSS_HP ? FULL : NORMAL;
                    var needBp = hp > EASY_BOSS_HP ? BP_FULL : BP_NORMAL;

                    if (ViewModel.Fever)
                    {
                        int quantity = needBp - ViewModel.BpValue;
                        if (quantity > 0 && quantity <= ViewModel.CanUseBpQuantity)
                        {
                            UseItem(ITEM_BP, INSTANT_MINI_BP, quantity);
                        }
                    }

                    if (ViewModel.BpValue >= needBp)
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
            var result = GetXHR("http://astrum.amebagames.com/_/event/furyraid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            PrintRaidBattleInfo(battleInfo);

            UpdateBpAfterRaidBattle(battleInfo);

            Delay(DELAY_SHORT);

            return battleInfo;
        }

        private void FuryRaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = PostXHR("http://astrum.amebagames.com/_/event/furyraid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            PrintBossBattleResult(battleResultInfo);
            Delay(DELAY_LONG);

        }

        private void FuryRaidBattleRescue(string raidId)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId }
            };
            PostXHR("http://astrum.amebagames.com/_/event/furyraid/battlerescue", values);
            Delay(DELAY_SHORT);
        }

        private void FuryRaidBattleResult(string raidId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/event/furyraid/battleresult?_id=" + Uri.EscapeDataString(raidId));
            RaidBattleInfo battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            var eventId = battleInfo.eventId;
            GetXHR("http://astrum.amebagames.com/_/event/furyraid/summary?_id=" + Uri.EscapeDataString(eventId));
        }

        public void LimitedRaid()
        {
            var raidInfo = LimitedRaidInfo();

            var loop = raidInfo.target != null && ViewModel.CanFullAttackForEvent;

            while (loop)
            {
                loop = LimitedRaidBattle(raidInfo.target._id);
            }
        }

        public LimitedRaidInfo LimitedRaidInfo()
        {
            var eventId = ViewModel.LimitedRaidEventId;
            string result = GetXHR("http://astrum.amebagames.com/_/event/limitedraid?_id=" + Uri.EscapeDataString(eventId));
            var raidInfo = JsonConvert.DeserializeObject<LimitedRaidInfo>(result);

            ViewModel.Fever = raidInfo.fever.gachaTicket != null;

            Delay(DELAY_SHORT);
            return raidInfo;
        }
        
        public bool LimitedRaidBattle(string raidId)
        {
            var battleInfo = LimitedRaidBattleInfo(raidId);

            if (battleInfo.isPlaying)
            {
                var hp = battleInfo.hp - battleInfo.totalDamage;

                var attackType = hp > EASY_BOSS_HP ? FULL : NORMAL;
                var needBp = hp > EASY_BOSS_HP ? BP_FULL : BP_NORMAL;

                if(ViewModel.Fever)
                {
                    int quantity = needBp - ViewModel.BpValue;
                    if (quantity > 0 && quantity <= ViewModel.CanUseBpQuantity)
                    {
                        UseItem(ITEM_BP, INSTANT_MINI_BP, quantity);
                    }
                }

                if (ViewModel.BpValue >= needBp)
                {
                    LimitedRaidBattleAttack(battleInfo._id, attackType);
                    return true;
                }
            }
            return false;
        }

        private RaidBattleInfo LimitedRaidBattleInfo(string raidId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/limitedraid/battle?_id=" + Uri.EscapeDataString(raidId));
            var battleInfo = JsonConvert.DeserializeObject<RaidBattleInfo>(result);

            PrintRaidBattleInfo(battleInfo);

            UpdateBpAfterRaidBattle(battleInfo);

            Delay(DELAY_SHORT);

            return battleInfo;
        }


        private void LimitedRaidBattleAttack(string raidId, string attackType)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", raidId },
                { "attackType", attackType }
            };
            //first
            var battleResult = PostXHR("http://astrum.amebagames.com/_/limitedraid/battle", values);
            var battleResultInfo = JsonConvert.DeserializeObject<BossBattleResultInfo>(battleResult);

            PrintBossBattleResult(battleResultInfo);
            Delay(DELAY_LONG);

        }

        public void GuildBattle()
        {
            Schedule schedule = FindSchedule();

            if (schedule != null)
            {
                var battleId = schedule._id;
                GuildBattleInfo battleInfo = GuildBattle(battleId);

                if (battleInfo.stamp.status)
                {
                    GuildBattleStamp(battleId);
                }

                GuildBattleChat();

                while (ViewModel.IsRunning)
                {
                    battleInfo = GuildBattle(battleId);

                    ViewModel.TpValue = battleInfo.status.tp.value;

                    if (battleInfo.status.tp.value >= 10)
                    {
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
                    else
                    {
                        //http://astrum.amebagames.com/_/guildbattle/tp?_id=B4e00c0644ed6fcfddd354c5cd714246994f6c7f9f3065b289bbd8ed1815d065d
                        TpInfo tpInfo = GuildBattleTpInfo(battleId);

                        if (tpInfo.normal.available)
                        {
                            GuildBattleTpNormal(battleId);
                        }
                        else if (tpInfo.chat.available)
                        {
                            GuildBattleTpChat(battleId);
                        }
                        else if (tpInfo.roulette.available)
                        {
                            GuildBattleRoulette(battleId);
                        }
                        else
                        {
                            // quest
                            if (ViewModel.StaminaValue >= 5)
                            {
                                TpQuest();
                            }
                            else
                            {
                                ViewModel.IsRunning = false;
                                return;
                            }
                        }
                    }
                }
            }
        }


        private Schedule FindSchedule()
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/lobby");
            GuildBattleLobbyInfo lobby = JsonConvert.DeserializeObject<GuildBattleLobbyInfo>(result);

            if (lobby.available && "start".Equals(lobby.status))
            {
                Access("/guildbattle&route=top&value=battle");
                return lobby.schedule.Find(item => "start".Equals(item.status));
            }
            return null;
        }

        private GuildBattleInfo GuildBattle(string battleId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle?_id=" + battleId);
            GuildBattleInfo battleInfo = JsonConvert.DeserializeObject<GuildBattleInfo>(result);

            this.Delay(DELAY_SHORT);

            return battleInfo;
        }

        private void GuildBattleStamp(string battleId)
        {
            var values = new Dictionary<string, string>
                {
                    { "_id", battleId }
                };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/stamp", values);
            this.Delay(DELAY_SHORT);
        }

        private void GuildBattleChat()
        {
            var values = new Dictionary<string, string>
            {
                { "stampId", "chat-stamp-004" }, { "type", "stamp" }
            };
            PostXHR("http://astrum.amebagames.com/_/guild/chat", values);
            this.Delay(DELAY_SHORT);
        }

        private GuildBattleCmdInfo GuildBattleCmd(string battleId, string type)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/cmd?_id=" + battleId + "&type=" + type);
            GuildBattleCmdInfo cmdInfo = JsonConvert.DeserializeObject<GuildBattleCmdInfo>(result);

            this.Delay(DELAY_SHORT);

            return cmdInfo;
        }


        private void GuildBattleCmd(string battleId, string abilityId, string cmd)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", battleId },
                { "abilityId", abilityId },
                { "cmd", cmd }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/cmd", values);

            this.Delay(DELAY_LONG);
        }


        private TpInfo GuildBattleTpInfo(string battleId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/tp?_id=" + Uri.EscapeDataString(battleId));
            TpInfo tpInfo = JsonConvert.DeserializeObject<TpInfo>(result);
            Delay(DELAY_SHORT);

            return tpInfo;
        }

        private void GuildBattleTpNormal(string battleId)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", battleId }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/normal", values);
            Delay(DELAY_SHORT);
        }

        private void GuildBattleTpChat(string battleId)
        {
            var values = new Dictionary<string, string>
            {
                { "_id", battleId }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/chat", values);
            Delay(DELAY_SHORT);
        }

        private void GuildBattleRoulette(string battleId)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette?_id=" + Uri.EscapeDataString(battleId));
            Roulette roulette = JsonConvert.DeserializeObject<Roulette>(result);

            Delay(DELAY_SHORT);

            int targetPosition = roulette.order.IndexOf(80);
            int position = roulette.initialPosition - targetPosition;


            var values = new Dictionary<string, string>
            {
                { "_id", battleId },
                { "position", position.ToString() }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette", values);
            Delay(DELAY_SHORT);
        }

        private void TpQuest()
        {
            Access("stage");

            var stage = EnterTpStage();

            while (ViewModel.IsRunning)
            {
                if (stage.status.tp.value >= 80)
                {
                    return;
                }
                if (stage.staminaEmpty)
                {
                    if (stage.items != null)
                    {
                        var item = stage.items.Find(e => INSTANT_HALF_STAMINA.Equals(e._id));
                        if (item.stock > ViewModel.MinStaminaStock)
                        {
                            UseItem(ITEM_STAMINA, INSTANT_HALF_STAMINA, 1);
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
                stage = ForwardTpStage();
            }
        }

        private StageInfo EnterTpStage()
        {
            var result = GetXHR("http://astrum.amebagames.com/_/stage/tp");
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            PrintStageInfo(stage);
            UpdateStageView(stage);
            Delay(DELAY_SHORT);

            return stage;
        }

        private StageInfo ForwardTpStage()
        {
            var values = new Dictionary<string, string>
                {
                   { "areaId", "recovery_tp" }
                };
            var result = PostXHR("http://astrum.amebagames.com/_/stage/tp", values);
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            PrintStageInfo(stage);
            UpdateStageView(stage);
            Delay(DELAY_SHORT);

            return stage;
        }

        private void PrintMypage(MypageInfo mypage)
        {
            string history = "";
            history += String.Format("   Name: {0} (L{1})", mypage.status.name, mypage.status.level) + Environment.NewLine;
            history += String.Format("  Total: {0}", mypage.total) + Environment.NewLine;
            history += String.Format("    ATK: {0},  DF: {1}", mypage.status.atk, mypage.status.df) + Environment.NewLine;
            history += String.Format("    MAT: {0}, MDF: {1}", mypage.status.mat, mypage.status.mdf) + Environment.NewLine;
            history += String.Format("  Guild: {0}, Rank: {1}", mypage.guild.name, mypage.guild.rank) + Environment.NewLine;

            ViewModel.History = history;
        }

        private void PrintStageInfo(StageInfo stage)
        {
            string history = "";
            history += String.Format("    Name: {0}[{1}] ({2})", stage.name, stage.stage, stage.isBossStage ? "BOSS" : stage.progress + "%") + Environment.NewLine;

            if (stage.status != null)
            {
                history += String.Format(" Stamina: {0} / {1}", stage.status.stamina.value, stage.status.stamina.max) + Environment.NewLine;
                history += String.Format("     EXP: {0} / {1}", stage.status.exp.value, stage.status.exp.max) + Environment.NewLine;
                history += String.Format("      BP: {0} / {1}", stage.status.bp.value, stage.status.bp.max) + Environment.NewLine;
                history += String.Format("      TP: {0} / {1}", stage.status.tp.value, stage.status.tp.max) + Environment.NewLine;
            }
            ViewModel.History = history;
        }

        private void PrintAreaBossInfo(AreaBossInfo boss)
        {
            string history = "";
            history += String.Format("   Name: {0}", boss.name) + Environment.NewLine;
            history += String.Format("   Area: {0}", boss.areaName) + Environment.NewLine;
            history += String.Format("     HP: {0} / {1}", boss.hp - boss.totalDamage, boss.hp) + Environment.NewLine;
            ViewModel.History = history;
        }

        private void PrintRaidBattleInfo(RaidBattleInfo battleInfo)
        {
            if (battleInfo.isPlaying)
            {
                string history = "";
                history += String.Format("   Name: {0} (L{1})", battleInfo.name, battleInfo.level) + Environment.NewLine;
                history += String.Format("   Rare: {0}", battleInfo.rare) + Environment.NewLine;
                history += String.Format("     HP: {0} / {1}", battleInfo.hp - battleInfo.totalDamage, battleInfo.hp) + Environment.NewLine;
                ViewModel.History = history;
            }
        }

        private void PrintBossBattleResult(BossBattleResultInfo resultInfo)
        {
            if (resultInfo.result != null)
            {
                string history = "";
                history += String.Format("  Result: {0}", resultInfo.result.resultType) + Environment.NewLine;
                history += String.Format("  BossHP: {0} / {1}", resultInfo.result.afterBoss.hp, resultInfo.result.afterBoss.maxHp) + Environment.NewLine;
                ViewModel.History = history;
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

                if (stage.status.furyraid != null && stage.status.furyraid.fever != null)
                {
                    var raidInfo = stage.status.furyraid;
                    ViewModel.Fever = raidInfo.fever.progress == 100;
                }


                if (stage.status.limitedraid != null && stage.status.limitedraid.fever != null)
                {
                    var raidInfo = stage.status.limitedraid;
                    ViewModel.Fever = raidInfo.fever.gachaTicket != null;
                }


            }
        }

        private void UpdateItemStock(ItemInfo item)
        {
            if (INSTANT_HALF_STAMINA.Equals(item._id))
            {
                ViewModel.StaminaHalfStock = item.stock;
            }
            else if (INSTANT_STAMINA.Equals(item._id))
            {
                ViewModel.StaminaStock = item.stock;
            }
            else if (INSTANT_MINI_BP.Equals(item._id))
            {
                ViewModel.BpMiniStock = item.stock;
            }
            else if (INSTANT_BP.Equals(item._id))
            {
                ViewModel.BpStock = item.stock;
            }
        }


        private void UpdateItemStock(UseItemResult item)
        {
            if (INSTANT_HALF_STAMINA.Equals(item._id))
            {
                ViewModel.StaminaHalfStock = item.stock.after;
                ViewModel.StaminaValue = item.value.after;
            }
            else if (INSTANT_STAMINA.Equals(item._id))
            {
                ViewModel.StaminaStock = item.stock.after;
                ViewModel.StaminaValue = item.value.after;
            }
            else if (INSTANT_MINI_BP.Equals(item._id))
            {
                ViewModel.BpMiniStock = item.stock.after;
                ViewModel.BpValue = item.value.after;
            }
            else if (INSTANT_BP.Equals(item._id))
            {
                ViewModel.BpStock = item.stock.after;
                ViewModel.BpValue = item.value.after;
            }
        }

        private void UpdateBpAfterRaidBattle(RaidBattleInfo battleInfo)
        {
            if (battleInfo.isPlaying)
            {
                ViewModel.BpValue = battleInfo.bpValue;
            }
        }
    }
}

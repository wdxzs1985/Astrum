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
using Astrum.Json.Gacha;
using Astrum.Json;
using Astrum.Json.Card;

namespace Astrum.Http
{

    public class AstrumClient : HttpClient
    {
        public const string XHR = "XMLHttpRequest";
        public const string PUT = "PUT";

        public const int INTERTAL = 100;
        public const int SECOND = 10 * INTERTAL;
        public const int MINUTE = 30 * SECOND;

        public const int DELAY_LONG = SECOND;
        public const int DELAY_SHORT = INTERTAL * 5;
        public const int NO_DELAY = 0;

        public const string ITEM_STAMINA = "stamina";
        public const string ITEM_BP = "bp";

        public const string INSTANT_HALF_STAMINA = "instant-half_stamina_potion";
        public const string INSTANT_STAMINA = "instant-stamina_potion";
        public const string INSTANT_BP = "instant-bp_ether";
        public const string INSTANT_MINI_BP = "instant-mini_bp_ether";

        public const string INSTANT_ABILITY_BOOK_GOLD = "instant-ability_book_gold";
        public const string INSTANT_ABILITY_BOOK_SILVER = "instant-ability_book_silver";
        public const string INSTANT_ABILITY_BOOK_BRONZE = "instant-ability_book_bronze";
        public const string INSTANT_STRENGTH_STATUE_GOLD = "instant-strength_statue_gold";
        public const string INSTANT_STRENGTH_STATUE_SILVER = "instant-strength_statue_silver";
        public const string INSTANT_STRENGTH_STATUE_BRONZE = "instant-strength_statue_bronze";

        public const string INSTANT_PLATINUM_GACHA_POINT = "instant-platinum_gacha_point";
        public const string INSTANT_RARE_RAID_MEDAL = "instant-rare_raid_medal";
        public const string INSTANT_RAID_MEDAL = "instant-raid_medal";

        public const string FULL = "full";
        public const string NORMAL = "normal";
        public const int BP_FULL = 3;
        public const int BP_NORMAL = 1;

        public const string FIRST = "first";
        public const string FIND = "find";
        public const string RESCUE = "rescue";


        public const int DEFAULT_STOCK = 999;
        public const int DEFAULT_KEEP_STAMINA = 100;

        public const int EASY_BOSS_HP = 2000000;

        public AstrumClient()
        {
            ViewModel = new ViewModel();

            ViewModel.IsQuestEnable = true;
            ViewModel.IsGuildBattleEnable = false;

            ViewModel.MinStaminaStock = DEFAULT_STOCK;
            ViewModel.MinBpStock = DEFAULT_STOCK;
            ViewModel.KeepStamina = DEFAULT_KEEP_STAMINA;
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
            lock (this)
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
        }

        protected string PostXHR(string url, Dictionary<string, object> values)
        {
            lock (this)
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
            }
            if (response.Headers.Get("X-Rtoken") != null)
            {
                xRtoken = response.Headers.Get("X-Rtoken");
            }
            if (response.Headers.Get("X-Utoken") != null)
            {
                xUtoken = response.Headers.Get("X-Utoken");
            }
            if (response.Headers.Get("X-Version") != null)
            {
                xVersion = response.Headers.Get("X-Version");
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

        public bool StartQuest()
        {
            this.Mypage();
            this.Item();

            return ViewModel.IsQuestEnable;
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

        public void Gift(int limited)
        {
            var hasGift = true;
            while (hasGift)
            {
                var giftInfo = CheckGift(limited);
                if (giftInfo.total > 0)
                {
                    ReceiveGift(limited);
                }
                else
                {
                    hasGift = false;
                }
            }
        }

        private GiftInfo CheckGift(int limited)
        {
            var url = string.Format("http://astrum.amebagames.com/_/gift?page=1&size=10&type=all&limited={0}", limited);
            var result = GetXHR(url);
            var giftInfo = JsonConvert.DeserializeObject<GiftInfo>(result);

            Access("gift");
            Delay(DELAY_SHORT);

            return giftInfo;
        }

        private void ReceiveGift(int limited)
        {
            var values = new Dictionary<string, object>
                    {
                       { "auto", "1" },
                       { "limited", limited },
                       { "type", "all" }
                    };
            var result = PostXHR("http://astrum.amebagames.com/_/gift", values);
            var giftResult = JsonConvert.DeserializeObject<GiftResult>(result);

            PrintGiftResult(giftResult);

            Delay(DELAY_SHORT);
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
                    switch(@event.type)
                    {
                        case "furyraid":
                            ViewModel.IsFuryRaidEnable = true;
                            ViewModel.FuryRaidEventId = @event._id;

                            ViewModel.IsFuryRaid = true;
                            FuryRaid();
                            break;
                        case "limitedraid":
                            ViewModel.IsLimitedRaidEnable = true;
                            ViewModel.LimitedRaidEventId = @event._id;
                            
                            ViewModel.IsLimitedRaid = true;

                            LimitedRaid();
                            break;
                        case "raid":
                            if(!ViewModel.Fever)
                            {
                                ViewModel.IsFuryRaid = false;
                                ViewModel.IsLimitedRaid = false;

                                Raid();
                            }
                            break;
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
                    ViewModel.IsFuryRaid = false;
                    ViewModel.IsLimitedRaid = false;

                    if (ViewModel.IsFuryRaidEnable)
                    {
                        ViewModel.IsFuryRaid = true;

                        if (stage.furyraid != null)
                        {
                            if (stage.furyraid.rare == 4)
                            {
                                ViewModel.CanFuryRaid = true;
                            }
                        }
                        else if (ViewModel.CanFuryRaid)
                        {
                            FuryRaid();
                            ViewModel.CanFuryRaid = false;
                            return;
                        }
                        else if (!ViewModel.Fever)
                        {
                            if (stage.status.furyraid.find != null)
                            {
                                if (stage.status.furyraid.find.isNew || ViewModel.CanFullAttack)
                                {
                                    FuryRaid();
                                    return;
                                }
                            }
                            if (stage.status.furyraid.rescue != null)
                            {
                                if (stage.status.furyraid.rescue.isNew)
                                {
                                    FuryRaid();
                                    return;

                                }
                            }
                        }
                    }

                    if (ViewModel.IsLimitedRaidEnable)
                    {
                        ViewModel.IsLimitedRaid = true;

                        var limitedRaidId = stage.status.limitedraid._id;
                        if (limitedRaidId != null)
                        {
                            if (ViewModel.CanFullAttackForEvent)
                            {
                                LimitedRaid();
                                return;
                            }
                        }
                    }
                    
                    if (stage.status.raid != null && !ViewModel.Fever)
                    {

                        ViewModel.IsFuryRaid = false;
                        ViewModel.IsLimitedRaid = false;

                        if (stage.status.raid.find != null)
                        {
                            if (stage.status.raid.find.isNew || ViewModel.CanFullAttack)
                            {
                                Raid();
                                return;
                            }
                        }
                        if (stage.status.raid.rescue != null)
                        {
                            if (stage.status.raid.rescue.isNew || ViewModel.CanFullAttack)
                            {
                                Raid();
                                return;
                            }
                        }
                    }


                    if (ViewModel.IsStaminaEmpty)
                    {
                        bool staminaGreaterThanKeep = ViewModel.StaminaValue >= ViewModel.KeepStamina;
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
                                return;
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
            UpdateStageView(stage.initial);
            Delay(DELAY_SHORT);

            return stage;
        }

        private StageInfo ForwardStage(string areaId)
        {
            var values = new Dictionary<string, object>
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
                var values = new Dictionary<string, object>
                {
                    { "itemId", item._id },
                    { "value", value }
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

            var values = new Dictionary<string, object>
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
                    var loop = battleInfo.isNew || ViewModel.CanFullAttack;
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
            var values = new Dictionary<string, object>
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
            var values = new Dictionary<string, object>
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
            FuryRaidInfo raidInfo = FuryRaidInfo();
            ViewModel.Fever = raidInfo.fever.progress == 100;

            raidInfo = FuryRaidBoss();
            if (raidInfo.find != null)
            {
                //ViewModel.FuryRaidFindList = raidInfo.find.list;
                foreach (var battleInfo in raidInfo.find.list)
                {
                    bool loop = battleInfo.rare == 4 || (!ViewModel.Fever && (battleInfo.isNew || ViewModel.CanFullAttack));
                    
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
                    var loop = battleInfo.isNew && !ViewModel.Fever;
                    while (loop)
                    {
                        loop = FuryRaidBattle(battleInfo._id);
                    }
                }
            }
        }

        public FuryRaidInfo FuryRaidInfo()
        {
            Access("furyraid");

            var eventId = ViewModel.FuryRaidEventId;
            string result = GetXHR("http://astrum.amebagames.com/_/event/furyraid?_id=" + Uri.EscapeDataString(eventId));
            FuryRaidInfo raidInfo = JsonConvert.DeserializeObject<FuryRaidInfo>(result);

            Delay(DELAY_SHORT);
            return raidInfo;
        }

        public FuryRaidInfo FuryRaidBoss()
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

                    if (battleInfo.rare == 4)
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
            var values = new Dictionary<string, object>
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
            var values = new Dictionary<string, object>
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
            var values = new Dictionary<string, object>
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


        public bool StartGuildBattle()
        {
            Schedule schedule = FindSchedule();
            if (schedule != null)
            {
                ViewModel.GuildBattleId = schedule._id;
                GuildBattleInfo battleInfo = GuildBattle(ViewModel.GuildBattleId);

                if (battleInfo.stamp.status)
                {
                    GuildBattleStamp(ViewModel.GuildBattleId);
                }

                GuildBattleChat();
                return ViewModel.IsGuildBattleEnable;
            } else
            {
                ViewModel.GuildBattleId = null;
                ViewModel.History = "没有工会战";
                return false;
            }
        }

        public void GuildBattle()
        {
            var battleId = ViewModel.GuildBattleId;

            while (ViewModel.TpValue >= 10 && ViewModel.IsRunning)
            {
                GuildBattleInfo battleInfo = GuildBattle(battleId);
                ViewModel.TpValue = battleInfo.status.tp.value;

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


            PrintGuildBattleInfo(battleInfo);
            UpdateGuildBattleStatus(battleInfo.status);

            this.Delay(DELAY_SHORT);

            return battleInfo;
        }

        private void GuildBattleStamp(string battleId)
        {
            var values = new Dictionary<string, object>
                {
                    { "_id", battleId }
                };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/stamp", values);
            this.Delay(DELAY_SHORT);
        }

        private void GuildBattleChat()
        {
            var values = new Dictionary<string, object>
            {
                { "stampId", "chat-stamp-004" },
                { "type", "stamp" }
            };
            PostXHR("http://astrum.amebagames.com/_/guild/chat", values);
            this.Delay(DELAY_SHORT);
        }

        private GuildBattleCmdInfo GuildBattleCmd(string battleId, string type)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/cmd?_id=" + battleId + "&type=" + type);
            GuildBattleCmdInfo cmdInfo = JsonConvert.DeserializeObject<GuildBattleCmdInfo>(result);

            //PrintCmdInfo(cmdInfo);
            UpdateGuildBattleStatus(cmdInfo.status);

            this.Delay(DELAY_SHORT);

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
            string result = PostXHR("http://astrum.amebagames.com/_/guildbattle/cmd", values);
            var cmdResult = JsonConvert.DeserializeObject<CmdResult>(result);


            if ("success".Equals(cmdResult.commandResult))
            {
                PrintGuildBattleCmdResult(cmdResult);
                UpdateGuildBattleStatus(cmdResult.battlestate.status);
            }


            this.Delay(DELAY_LONG);
        }


        private TpInfo GuildBattleTpInfo()
        {
            var battleId = ViewModel.GuildBattleId;

            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/tp?_id=" + Uri.EscapeDataString(battleId));
            TpInfo tpInfo = JsonConvert.DeserializeObject<TpInfo>(result);
            Delay(DELAY_SHORT);
            
            ViewModel.IsTpNormalAvailable = tpInfo.normal.available;
            ViewModel.IsTpChatAvailable = tpInfo.chat.available;
            ViewModel.IsTpRouletteAvailable = tpInfo.roulette.available;

            return tpInfo;
        }

        public void GuildBattleTpNormal()
        {

            var battleId = ViewModel.GuildBattleId;
            var values = new Dictionary<string, object>
            {
                { "_id", battleId }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/normal", values);

            ViewModel.History = "回复TP";
            ViewModel.IsTpNormalAvailable = false;

            Delay(DELAY_SHORT);
        }

        public void GuildBattleTpChat()
        {
            var battleId = ViewModel.GuildBattleId;
            var values = new Dictionary<string, object>
            {
                { "_id", battleId }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/chat", values);

            ViewModel.History = "回复TP";
            ViewModel.IsTpChatAvailable = false;

            Delay(DELAY_SHORT);
        }

        public void GuildBattleTpRoulette()
        {
            var battleId = ViewModel.GuildBattleId;
            var result = GetXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette?_id=" + Uri.EscapeDataString(battleId));
            Roulette roulette = JsonConvert.DeserializeObject<Roulette>(result);

            Delay(DELAY_SHORT);

            int targetPosition = roulette.order.IndexOf(80);
            int position = roulette.initialPosition - targetPosition;


            var values = new Dictionary<string, object>
            {
                { "_id", battleId },
                { "position", position }
            };
            PostXHR("http://astrum.amebagames.com/_/guildbattle/tp/roulette", values);

            ViewModel.History = "回复TP";
            ViewModel.IsTpRouletteAvailable = false;

            Delay(DELAY_SHORT);
        }

        private void TpQuest()
        {
            Access("stage");

            var stage = EnterTpStage();

            while (ViewModel.IsRunning)
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
            var result = GetXHR("http://astrum.amebagames.com/_/stage/tp");
            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            PrintStageInfo(stage);
            UpdateStageView(stage);
            Delay(DELAY_SHORT);

            return stage;
        }

        private StageInfo ForwardTpStage()
        {
            var values = new Dictionary<string, object>
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


        public bool StartGacha()
        {
            initGachaList();
            ViewModel.History = "";
            return true;
        }
        
        private void initGachaList()
        {
            var gachaList = new List<GachaInfo>();
            var stockMap = new Dictionary<string, int>();

            initGachaType(gachaList, stockMap, "normal");
            initGachaType(gachaList, stockMap, "raid");
            initGachaType(gachaList, stockMap, "platinum");

            ViewModel.GachaList = gachaList;
        }

        private void initGachaType(List<GachaInfo> gachaList, Dictionary<string, int> stockMap, string type)
        {
            var gacha = GachaListInfo(type);
            stockMap["coin"] = gacha.stock.coin;
            stockMap["gacha"] = gacha.stock.gacha;

            if (gacha.stock.ticket != null)
            {
                foreach (var key in gacha.stock.ticket.Keys)
                {
                    stockMap[key] = gacha.stock.ticket[key];
                }
            }

            foreach (var item in gacha.list)
            {
                    var key = "ticket".Equals(item.price.type) ? item.price._id : item.price.type;
                    
                    if(!"coin".Equals(key))
                    {
                        item.stock = stockMap[key];
                        gachaList.Add(item);
                    }
            }
        }
        

        public void Gacha(string gachaId, bool sequence)
        {
            var result = GachaResult(gachaId, sequence);

            PrintGachaResult(result);
            UpdataGachaResult(result);

            initGachaList();
        }

        private GachaList GachaListInfo(string type)
        {
            var result = GetXHR("http://astrum.amebagames.com/_/gacha?type=" + type);
            return JsonConvert.DeserializeObject<GachaList>(result);
        }

        private GachaResult GachaResult(string _id, bool sequence)
        {
            var values = new Dictionary<string, object>
                {
                   { "_id", _id }
                };

            if (sequence)
            {
                values.Add("sequence", true);
            }

            var result = PostXHR("http://astrum.amebagames.com/_/gacha", values);
            return JsonConvert.DeserializeObject<GachaResult>(result);
        }

        public void StartTraining()
        {
            StartTraining(ViewModel.TrainingBaseId == null ? "" : ViewModel.TrainingBaseId);
        }

        public void StartTraining(string baseId)
        {
            RaiseInfo raiseInfo = RaiseSearch(baseId, 1);

            ViewModel.CardQuantity = raiseInfo.card.value;
            ViewModel.CardMax = raiseInfo.card.max;

            ViewModel.TrainingBaseId = raiseInfo.@base._id;
            ViewModel.TrainingBaseRare = raiseInfo.@base.rare;
            ViewModel.TrainingBaseName = raiseInfo.@base.name;
            ViewModel.TrainingBaseLevel = raiseInfo.@base.level;
            ViewModel.TrainingBaseMaxLevel = raiseInfo.@base.maxLevel;
            ViewModel.TrainingBaseAbilityLevel = raiseInfo.@base.abilityLevel;
            ViewModel.TrainingBaseMaxAbilityLevel = raiseInfo.@base.maxAbilityLevel;
            ViewModel.TrainingBaseExpGrowth = raiseInfo.@base.growth.exp;
            ViewModel.TrainingBaseAbilityGrowth = raiseInfo.@base.growth.ability;
            
            RaiseInfo raiseItemInfo = RaiseItem(ViewModel.TrainingBaseId);
            ViewModel.AbilityBookGoldStock = 0;
            ViewModel.AbilityBookGoldAvailable = 0;
            ViewModel.AbilityBookSilverStock = 0;
            ViewModel.AbilityBookSilverAvailable = 0;
            ViewModel.AbilityBookBronzeStock = 0;
            ViewModel.AbilityBookBronzeAvailable = 0;
            ViewModel.StrengthStatueGoldStock = 0;
            ViewModel.StrengthStatueGoldAvailable = 0;
            ViewModel.StrengthStatueSilverStock = 0;
            ViewModel.StrengthStatueSilverAvailable = 0;
            ViewModel.StrengthStatueBronzeStock = 0;
            ViewModel.StrengthStatueBronzeAvailable = 0;


            if (raiseItemInfo.items != null)
            {
                if (raiseItemInfo.items.ability != null)
                {
                    foreach (var item in raiseItemInfo.items.ability)
                    {
                        switch (item._id)
                        {
                            case INSTANT_ABILITY_BOOK_GOLD:
                                ViewModel.AbilityBookGoldStock = item.stock;
                                ViewModel.AbilityBookGoldAvailable = item.available;
                                break;
                            case INSTANT_ABILITY_BOOK_SILVER:
                                ViewModel.AbilityBookSilverStock = item.stock;
                                ViewModel.AbilityBookSilverAvailable = item.available;
                                break;
                            case INSTANT_ABILITY_BOOK_BRONZE:
                                ViewModel.AbilityBookBronzeStock = item.stock;
                                ViewModel.AbilityBookBronzeAvailable = item.available;
                                break;
                        }
                    }
                }

                if (raiseItemInfo.items.exp != null)
                {
                    foreach (var item in raiseItemInfo.items.exp)
                    {
                        switch (item._id)
                        {
                            case INSTANT_STRENGTH_STATUE_GOLD:
                                ViewModel.StrengthStatueGoldStock = item.stock;
                                ViewModel.StrengthStatueGoldAvailable = item.available;
                                break;
                            case INSTANT_STRENGTH_STATUE_SILVER:
                                ViewModel.StrengthStatueSilverStock = item.stock;
                                ViewModel.StrengthStatueSilverAvailable = item.available;
                                break;
                            case INSTANT_STRENGTH_STATUE_BRONZE:
                                ViewModel.StrengthStatueBronzeStock = item.stock;
                                ViewModel.StrengthStatueBronzeAvailable = item.available;
                                break;
                        }
                    }
                }
            }
        }
        private RaiseInfo RaiseSearch(string baseId, int rare)
        {
            var page = 1;
            var size = 20;
            var target = "time";
            var sort = "desc";

            var url = string.Format("http://astrum.amebagames.com/_/raise?page={0}&size={1}&base={2}&target={3}&sort={4}&rare={5}&level1=true", page, size, Uri.EscapeDataString(baseId), target, sort, rare);

            var result = GetXHR(url);

            return JsonConvert.DeserializeObject<RaiseInfo>(result);
        }

        private RaiseInfo RaiseItem(string baseId)
        {
            var url = string.Format("http://astrum.amebagames.com/_/raise?type=item&base={0}",  Uri.EscapeDataString(baseId));

            var result = GetXHR(url);

            return JsonConvert.DeserializeObject<RaiseInfo>(result);
        }


        public bool ExecuteRaiseNormal()
        {
            var baseId = ViewModel.TrainingBaseId;
            RaiseInfo raiseInfo = RaiseSearch(baseId, 1);
            var type = "card";

            if(raiseInfo.total > 0)
            {
                IEnumerable<string> materials = from card in raiseInfo.list
                                                select card._id;

                ExecuteRaise(baseId, materials, type);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExecuteRaiseRare()
        {
            var baseId = ViewModel.TrainingBaseId;
            RaiseInfo raiseInfo = RaiseSearch(baseId, 2);
            var type = "card";

            if (raiseInfo.total > 0)
            {
                IEnumerable<string> materials = from card in raiseInfo.list
                                                select card._id;

                ExecuteRaise(baseId, materials, type);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExecuteRaiseItem(string itemId, int quantity)
        {
            var baseId = ViewModel.TrainingBaseId;
            var materials = new Dictionary<string, object>
                {
                   { itemId, quantity }
                };
            var type = "item";

            ExecuteRaise(baseId, materials, type);
            return true;
        }

        public void ExecuteRaise(string baseId, object materials,string type)
        {
            var values = new Dictionary<string, object>
                {
                   { "base", baseId },
                   { "materials", materials },
                   { "type", type }
                };

            PostXHR("http://astrum.amebagames.com/_/raise/execute", values);
        }

        public void TrainingBase()
        {
            var page = 1;
            var size = 150;
            
            var target = "rare";
            var sort = "desc";

            var url = string.Format("http://astrum.amebagames.com/_/raise/base?page={0}&size={1}&target={2}&sort={3}&level1=false&inParty=false", page, size, target, sort);

            var result = GetXHR(url);

            RaiseInfo search = JsonConvert.DeserializeObject<RaiseInfo>(result);

            if(search.total > 0)
            {
                ViewModel.IsTrainingBaseEnable = true;
                ViewModel.TrainingBaseList = search.list;
            }
            else
            {
                ViewModel.IsTrainingBaseEnable = false;
            }
        }


        private void PrintMypage(MypageInfo mypage)
        {
            string history = "";
            history += String.Format("　　角色：{0} (L{1})", mypage.status.name, mypage.status.level) + Environment.NewLine;
            history += String.Format("　总战力：{0}", mypage.total) + Environment.NewLine;
            history += String.Format("　　攻击：{0}，　　防御：{1}", mypage.status.atk, mypage.status.df) + Environment.NewLine;
            history += String.Format("必杀攻击：{0}，必杀防御：{1}", mypage.status.mat, mypage.status.mdf) + Environment.NewLine;
            history += String.Format("所属工会：{0}, 工会排行：{1}", mypage.guild.name, mypage.guild.rank) + Environment.NewLine;

            ViewModel.History = history;
        }



        private void PrintGiftResult(GiftResult giftResult)
        {
            string history = "";
            if(giftResult.item > 0)
            {
                history += String.Format("　　　获得道具：{0}", giftResult.item) + Environment.NewLine;
            }
            if (giftResult.lilu > 0)
            {
                history += String.Format("　　　获得ルピ：{0}", giftResult.lilu) + Environment.NewLine;
            }
            if (giftResult.card > 0)
            {
                history += String.Format("　　　获得少女：{0}", giftResult.card) + Environment.NewLine;
            }
            if (giftResult.enhance != null)
            {
                if (giftResult.enhance.strength > 0)
                {
                    history += String.Format("　　获得强化像：{0}", giftResult.enhance.strength) + Environment.NewLine;
                }
                if (giftResult.enhance.limitbreak > 0)
                {
                    history += String.Format("　获得开花结晶：{0}", giftResult.enhance.limitbreak) + Environment.NewLine;
                }
            }

            if (giftResult.gacha != null)
            {
                if (giftResult.gacha.ContainsKey(INSTANT_PLATINUM_GACHA_POINT))
                {
                    var value = giftResult.gacha[INSTANT_PLATINUM_GACHA_POINT].value;
                    history += String.Format("　　　获得星钻：{0}", value) + Environment.NewLine;
                }
                if (giftResult.gacha.ContainsKey(INSTANT_RARE_RAID_MEDAL))
                {
                    var value = giftResult.gacha[INSTANT_RARE_RAID_MEDAL].value;
                    history += String.Format("获得稀有魔星币：{0}", value) + Environment.NewLine;
                }
                if (giftResult.gacha.ContainsKey(INSTANT_RAID_MEDAL))
                {
                    var value = giftResult.gacha[INSTANT_RAID_MEDAL].value;
                    history += String.Format("　　获得魔星币：{0}", value) + Environment.NewLine;
                }
            }

            ViewModel.History = history;
        }

        private void PrintStageInfo(StageInfo stage)
        {
            string history = "";
            history += String.Format("场景：{0}[{1}] ({2})", stage.name, stage.stage, stage.isBossStage ? "BOSS" : stage.progress + "%") + Environment.NewLine;

            if (stage.status != null)
            {
                history += String.Format("体力：{0} / {1}", stage.status.stamina.value, stage.status.stamina.max) + Environment.NewLine;
                history += String.Format("经验：{0} / {1}", stage.status.exp.value, stage.status.exp.max) + Environment.NewLine;
                history += String.Format("　BP：{0} / {1}", stage.status.bp.value, stage.status.bp.max) + Environment.NewLine;
                history += String.Format("　TP：{0} / {1}", stage.status.tp.value, stage.status.tp.max) + Environment.NewLine;
            }
            ViewModel.History = history;
        }

        private void PrintAreaBossInfo(AreaBossInfo boss)
        {
            string history = "";
            history += String.Format("{0}的{1}出现了！", boss.areaName, boss.name) + Environment.NewLine;
            history += String.Format("血量：{0} / {1}", boss.hp - boss.totalDamage, boss.hp) + Environment.NewLine;
            ViewModel.History = history;
        }

        private void PrintRaidBattleInfo(RaidBattleInfo battleInfo)
        {
            if (battleInfo.isPlaying)
            {
                string history = "";

                string rare = "";
                switch(battleInfo.rare)
                {
                    case 1:
                        rare = "初级魔星兽";
                        break;
                    case 2:
                        rare = "中级魔星兽";
                        break;
                    case 3:
                        rare = "上级魔星兽";
                        break;
                    case 4:
                        rare = "星兽王";
                        break;
                    default:
                        rare = "魔星兽";
                        break;
                }

                history += String.Format("{0}({1} L{2})出现了", rare, battleInfo.name, battleInfo.level) + Environment.NewLine;
                history += String.Format("血量: {0} / {1}", battleInfo.hp - battleInfo.totalDamage, battleInfo.hp) + Environment.NewLine;
                ViewModel.History = history;
            }
        }

        private void PrintBossBattleResult(BossBattleResultInfo resultInfo)
        {
            if (resultInfo.result != null)
            {
                string history = "";
                history += String.Format("战斗结束：{0}", resultInfo.result.resultType) + Environment.NewLine;
                history += String.Format("BOSS血量：{0} / {1}", resultInfo.result.afterBoss.hp, resultInfo.result.afterBoss.maxHp) + Environment.NewLine;
                ViewModel.History = history;
            }
        }


        private void PrintGuildBattleInfo(GuildBattleInfo battleInfo)
        {
            string history = "";

            foreach(var guild in battleInfo.guilds)
            {
                history += guild.name + Environment.NewLine;
                history += String.Format("普通攻击：{0}", guild.combo.attack.count) + Environment.NewLine;
                history += String.Format("远程攻击：{0}", guild.combo.remote.count) + Environment.NewLine;
                history += String.Format("必杀攻击：{0}", guild.combo.special.count) + Environment.NewLine;
                history += String.Format("　　应援：{0}", guild.combo.yell.count) + Environment.NewLine;
                history += Environment.NewLine;
            }
            ViewModel.History = history;
        }

        private void PrintGuildBattleCmdResult(CmdResult result)
        {
            var status = result.battlestate.status;

            string history = "";
            
            history += String.Format("{0} (Combo {1})", result.cmd.name, result.history.combo) + Environment.NewLine;

            ViewModel.History = history;
        }

        private void PrintGachaResult(GachaResult result)
        {
            string history = "";
            foreach (var item in result.list)
            {
                
                if ("item".Equals(item.type))
                {
                    history += String.Format("{0} x {1}", item.name, item.value) + Environment.NewLine;
                }
                else if ("card".Equals(item.type))
                {
                    string rare = "";
                    switch (item.rare)
                    {
                        case 4:
                            rare = "[欧皇]";
                            break;
                        case 3:
                            rare = "[脸帝]";
                            break;
                        case 2:
                            rare = "[狗粮]";
                            break;
                        case 1:
                            rare = "[渣渣]";
                            break;
                        default:
                            rare = "[????]";
                            break;
                    }

                    history += String.Format("{0}{1} x {2}", rare, item.name, item.value) + Environment.NewLine;
                }
            }
            ViewModel.History = history;
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

            ViewModel.CardQuantity = mypage.status.card_quantity;
            ViewModel.CardMax = mypage.status.card_max;
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

                if (ViewModel.IsFuryRaidEnable)
                {
                    ViewModel.Fever = stage.status.furyraid != null && stage.status.furyraid.fever != null;
                }
                
                if (ViewModel.IsLimitedRaidEnable)
                {
                    ViewModel.Fever = stage.status.limitedraid != null && stage.status.limitedraid.fever != null;
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



        private void UpdateGuildBattleStatus(GuildBattleStatus status)
        {

            ViewModel.TpValue = status.tp.value;
            ViewModel.TpMax = status.tp.max;

        }


        private void UpdataGachaResult(GachaResult result)
        {
            ViewModel.CardQuantity = result.card.value;
            ViewModel.CardMax = result.card.max;
        }
    }
}

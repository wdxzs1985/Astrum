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
using Astrum.Json.Breeding;
using Astrum.Handler;

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
        public const string INSTANT_PLATINUM_GACHA_TICKET = "instant-platinum_gacha_ticket";
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

        public ViewModel ViewModel { get; set; }
        private QuestHandler _questHandler;
        private RaidHandler _raidHandler;
        private FuryRaidHandler _furyRaidHandler;
        private LimitedRaidHandler _limitedRaidHandler;
        private BreedingHandler _breedingHandler;
        private GuildBattleHandler _guildBattleHandler;
        private MypageHandler _mypageHandler;
        private GiftHandler _giftHandler;
        private ItemHandler _itemHandler;
        private GachaHandler _gachaHandler;
        private TraningHandler _trainingHandler;

        private string xGroup = "a";
        private string xRtoken = "undefined";
        private string xUtoken = "undefined";
        private string xVersion = "undefined";

        private Random seed = new Random();

        public AstrumClient()
        {
            ViewModel = new ViewModel();
            _questHandler = new QuestHandler(this);
            _raidHandler = new RaidHandler(this);
            _furyRaidHandler = new FuryRaidHandler(this);
            _limitedRaidHandler = new LimitedRaidHandler(this);
            _breedingHandler = new BreedingHandler(this);
            _guildBattleHandler = new GuildBattleHandler(this);
            _mypageHandler = new MypageHandler(this);
            _giftHandler = new GiftHandler(this);
            _itemHandler = new ItemHandler(this);
            _gachaHandler = new GachaHandler(this);
            _trainingHandler = new TraningHandler(this);

            ViewModel.IsQuestEnable = true;
            ViewModel.IsGuildBattleEnable = false;

            ViewModel.MinStaminaStock = DEFAULT_STOCK;
            ViewModel.MinBpStock = DEFAULT_STOCK;
            ViewModel.KeepStamina = DEFAULT_KEEP_STAMINA;
        }
        
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

        public void DelayShort()
        {
            this.Delay(DELAY_SHORT);
        }

        public void DelayLong()
        {
            this.Delay(DELAY_LONG);
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

        public string GetXHR(string url)
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

        public string PostXHR(string url, Dictionary<string, object> values)
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

        public bool DownloadUserAvatar(CardInfo card)
        {
            var url = buildSdUrl(card);

            var dirname = "cache";
            if (!Directory.Exists(dirname))
            {
                Directory.CreateDirectory(dirname);
            }

            var md5 = card.md5.sd;
            var filename = String.Format("{0}-avatar.png", md5);
            var pathString = Path.Combine(dirname, filename);

            if (File.Exists(pathString))
            {
                return true;
            }
            
            return this.DownloadBinary(url, pathString);
        }

        public bool DownloadCardThumb(CardInfo card)
        {
            var url = buildThumbUrl(card);

            var dirname = "cache";
            if (!Directory.Exists(dirname))
            {
                Directory.CreateDirectory(dirname);
            }

            var md5 = card.md5.image;
            var filename = String.Format("{0}-thumb.png", md5);
            var pathString = Path.Combine(dirname, filename);

            if(File.Exists(pathString))
            {
                return true;
            }

            return this.DownloadBinary(url, pathString);
        }

        private string buildSdUrl(CardInfo card)
        {
            var id = card._id;
            var md5 = card.md5.sd;
            
            return buildImageUrl(id, md5, "http://aos.a4c.jp/paris/p/stat/a/{0}/s/animation/sd/chara/{1}/{2}/{3}/{4}/{5}/static_standby/3/{6}-static_standby-3_{7}.png");
        }

        private string buildThumbUrl(CardInfo card)
        {
            var id = card._id;
            var md5 = card.md5.image;

            return buildImageUrl(id, md5, "http://aos.a4c.jp/paris/p/stat/a/{0}/s/card/{1}/{2}/{3}/{4}/{5}/thumb/10/{6}-thumb-10_{7}.png");
        }

        private string buildImageUrl(string id, string md5, string urlFormat)
        {
            var indexOfAt = id.IndexOf("@");
            id = id.Remove(indexOfAt);

            var parts = id.Split('-');

            var type = parts[0];
            var race = parts[1];
            var weapon = parts[2];
            var code = parts[3];
            var prefix = code.Substring(0, 2);
            
            return String.Format(urlFormat, this.xVersion, type, race, weapon, prefix, code, md5, 20);
        }

        
        public void Mypage()
        {
            _mypageHandler.Run();
        }

        public void EventStatus()
        {
            _mypageHandler.EventStatus();
        }

        public void Profile()
        {
            _mypageHandler.Profile();
        }

        public void Gift()
        {
            _giftHandler.Run();
        }

        public bool StartQuest()
        {
            return _questHandler.Start();
        }


        public void Quest()
        {
            this._questHandler.Run();
        }
        
        public void Raid()
        {
            _raidHandler.Run();
        }


        public void FuryRaid()
        {
            _furyRaidHandler.Run();
        }


        public void LimitedRaid()
        {
            _limitedRaidHandler.Run();
        }
        
        public bool StartGuildBattle()
        {
            return _guildBattleHandler.Start();
        }

        public void GuildBattle()
        {
            _guildBattleHandler.Run();

        }

        internal void GuildBattleTpChat()
        {
            _guildBattleHandler.GuildBattleTpChat();
        }

        internal void GuildBattleTpNormal()
        {
            _guildBattleHandler.GuildBattleTpNormal();
        }

        internal void GuildBattleTpRoulette()
        {
            _guildBattleHandler.GuildBattleTpRoulette();
        }

        public void Breeding()
        {
            _breedingHandler.Run();
        }
        
        public void Item()
        {
            _itemHandler.Run();
        }

        public void UseItem(string type, string itemId, int value)
        {
            _itemHandler.UseItem(type, itemId, value);
        }

        public bool StartGacha()
        {
            return _gachaHandler.Start();
        }
        
        public void Gacha(string gachaId, bool sequence)
        {
            _gachaHandler.Run(gachaId, sequence);
        }
        

        public void StartTraining()
        {
            _trainingHandler.Start();
        }

        public void StartTraining(string baseId)
        {
            _trainingHandler.StartTraining(baseId);
        }

        public bool ExecuteRaiseNormal()
        {
            return _trainingHandler.ExecuteRaiseNormal();
        }

        public bool ExecuteRaiseRare()
        {
           return _trainingHandler.ExecuteRaiseRare();
        }

        public bool ExecuteRaiseItem(string itemId, int quantity)
        {
            return _trainingHandler.ExecuteRaiseItem(itemId, quantity);
        }

        public void TrainingBase()
        {
            _trainingHandler.TrainingBase();
        }
    }
}

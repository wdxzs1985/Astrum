using Astrum.Http;
using Astrum.Json.Card;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class TraningHandler
    {
        private AstrumClient _client = null;

        public TraningHandler(AstrumClient client)
        {
            _client = client;
        }


        public bool Start()
        {
            var baseId = _client.ViewModel.TrainingBaseId == null ? "" : _client.ViewModel.TrainingBaseId;
            StartTraining(baseId);

            return _client.ViewModel.IsTrainingEnable;
        }



        public void StartTraining(string baseId)
        {
            RaiseInfo raiseInfo = RaiseSearch(baseId, 1);
            InfoUpdater.UpdateRaiseInfo(raiseInfo, _client.ViewModel);

            RaiseInfo raiseItemInfo = RaiseItem(_client.ViewModel.TrainingBaseId);
            InfoUpdater.UpdateRaiseItemInfo(raiseItemInfo, _client.ViewModel);
        }


        private RaiseInfo RaiseSearch(string baseId, int rare)
        {
            var page = 1;
            var size = 20;
            var target = "time";
            var sort = "desc";

            var url = string.Format("http://astrum.amebagames.com/_/raise?page={0}&size={1}&base={2}&target={3}&sort={4}&rare={5}&level1=true", page, size, Uri.EscapeDataString(baseId), target, sort, rare);

            var result = _client.GetXHR(url);

            return JsonConvert.DeserializeObject<RaiseInfo>(result);
        }


        private RaiseInfo RaiseItem(string baseId)
        {
            var url = string.Format("http://astrum.amebagames.com/_/raise?type=item&base={0}", Uri.EscapeDataString(baseId));
            var result = _client.GetXHR(url);

            return JsonConvert.DeserializeObject<RaiseInfo>(result);
        }


        public void TrainingBase()
        {
            var page = 1;
            var size = 150;

            var target = "rare";
            var sort = "desc";

            var url = string.Format("http://astrum.amebagames.com/_/raise/base?page={0}&size={1}&target={2}&sort={3}&level1=false&inParty=false", page, size, target, sort);
            var result = _client.GetXHR(url);

            RaiseInfo search = JsonConvert.DeserializeObject<RaiseInfo>(result);

            if (search.total > 0)
            {
                _client.ViewModel.IsTrainingBaseEnable = true;
                _client.ViewModel.TrainingBaseList = search.list;
            }
            else
            {
                _client.ViewModel.IsTrainingBaseEnable = false;
            }
        }


        public void ExecuteRaise(string baseId, object materials, string type)
        {
            var values = new Dictionary<string, object>
                {
                   { "base", baseId },
                   { "materials", materials },
                   { "type", type }
                };

            _client.PostXHR("http://astrum.amebagames.com/_/raise/execute", values);
        }


        public bool ExecuteRaiseNormal()
        {
            var baseId = _client.ViewModel.TrainingBaseId;
            RaiseInfo raiseInfo = RaiseSearch(baseId, 1);
            var type = "card";

            if (raiseInfo.total > 0)
            {
                var materials = from card in raiseInfo.list
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
            var baseId = _client.ViewModel.TrainingBaseId;
            RaiseInfo raiseInfo = RaiseSearch(baseId, 2);
            var type = "card";

            if (raiseInfo.total > 0)
            {
                var materials = from card in raiseInfo.list
                                select card._id;

                ExecuteRaise(baseId, materials, type);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExecuteSellNormal()
        {
            //GET
            //http://astrum.amebagames.com/_/card/catalog?page=1&size=20&type=sell
            var search = SellSearch();

            if (search.total > 0)
            {
                var sellList = search.list.Where(card => card.rare == 1);

                if(sellList.Count() > 0)
                {
                    var cardIds = from card in sellList
                                  select card._id;

                    ExecuteSell(cardIds);
                }
            }
            return true;
        }

        private CardSearchInfo SellSearch()
        {
            var page = 1;
            var size = 20;
            var target = "rare";

            var url = string.Format("http://astrum.amebagames.com/_/card/catalog?page={0}&size={1}&type=sell&uppersr=false&target={2}&sort=asc&atr=all&display=lilu", page, size, target);

            var result = _client.GetXHR(url);

            return JsonConvert.DeserializeObject<CardSearchInfo>(result);
        }


        public void ExecuteSell(object cardIds)
        {
            var values = new Dictionary<string, object>
                {
                   { "cardIds", cardIds }
                };

            _client.PostXHR("http://astrum.amebagames.com/_/card/sell", values);
        }


        public bool ExecuteRaiseItem(string itemId, int quantity)
        {
            var baseId = _client.ViewModel.TrainingBaseId;
            var materials = new Dictionary<string, object>
                {
                   { itemId, quantity }
                };
            var type = "item";

            ExecuteRaise(baseId, materials, type);
            return true;
        }
    }
}

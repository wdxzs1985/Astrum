using Astrum.Http;
using Astrum.Json.Item;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class ItemHandler
    {
        private AstrumClient _client = null;

        public ItemHandler(AstrumClient client)
        {
            _client = client;
        }

        public void Run()
        {
            var responseString = _client.GetXHR("http://astrum.amebagames.com/_/item");
            var itemList = JsonConvert.DeserializeObject<ItemList>(responseString);

            foreach (var item in itemList.list)
            {
                InfoUpdater.UpdateItemStock(item, _client.ViewModel);
            }

            _client.Access("item");
        }

        public void UseItem(string type, string itemId, int value)
        {
            var responseString = _client.GetXHR("http://astrum.amebagames.com/_/item/common?type=" + type);
            var itemList = JsonConvert.DeserializeObject<ItemList>(responseString);

            var item = itemList.list.Find(e => itemId.Equals(e._id));
            if (item.stock >= value)
            {
                var values = new Dictionary<string, object>
                {
                    { "itemId", item._id },
                    { "value", value }
                };
                string result = _client.PostXHR("http://astrum.amebagames.com/_/item/common", values);
                var useItemResult = JsonConvert.DeserializeObject<UseItemResult>(result);

                InfoUpdater.UpdateItemStock(useItemResult, _client.ViewModel);

                _client.DelayShort();
            }
        }
    }
}

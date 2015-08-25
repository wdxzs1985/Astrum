using Astrum.Http;
using Astrum.Json.Gift;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class GiftHandler
    {
        private AstrumClient _client = null;

        public GiftHandler(AstrumClient client)
        {
            _client = client;
        }

        public void Run()
        {
            foreach(var limited in new int[0, 1, 2])
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

        }

        private GiftInfo CheckGift(int limited)
        {
            var url = string.Format("http://astrum.amebagames.com/_/gift?page=1&size=10&type=all&limited={0}", limited);
            var result = _client.GetXHR(url);
            var giftInfo = JsonConvert.DeserializeObject<GiftInfo>(result);

            _client.Access("gift");
            _client.DelayShort();

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
            var result = _client.PostXHR("http://astrum.amebagames.com/_/gift", values);
            var giftResult = JsonConvert.DeserializeObject<GiftResult>(result);

            InfoPrinter.PrintGiftResult(giftResult, _client.ViewModel);

            _client.DelayShort();
        }
    }
}

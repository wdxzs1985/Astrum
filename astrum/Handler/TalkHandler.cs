using Astrum.Http;
using Astrum.Json.Talk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    public class TalkHandler
    {
        private AstrumClient _client;

        public TalkHandler(AstrumClient client)
        {
            _client = client;
        }

        public void Run()
        {
            TalkListInfo talkList = TalkList();
            _client.ViewModel.TalkList = talkList.list;
            _client.Access("talk");
        }

        private TalkListInfo TalkList()
        {
            var url = string.Format("http://astrum.amebagames.com/_/talk?size={0}", 10);
            var result = _client.GetXHR(url);
            var info = JsonConvert.DeserializeObject<TalkListInfo>(result);
            
            _client.DelayShort();
            return null;
        }


    }
}

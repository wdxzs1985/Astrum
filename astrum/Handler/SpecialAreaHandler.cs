using Astrum.Http;
using Astrum.Json.Breeding;
using Astrum.Json.Raid;
using Astrum.Json.Stage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    public class SpecialAreaHandler : QuestHandler
    {
        

        public SpecialAreaHandler(AstrumClient client) : base(client)
        {
            
        }

        public void CheckExtraMap()
        {
            MapInfo maps = ExtraMap();
            AreaInfo area = maps.list.Find(a => (a.isNew || a.order == 1) && a.stock > 0);

            if(area == null)
            {
                _client.ViewModel.IsSpecialAreaEnable = false;
            }
        }

        protected MapInfo ExtraMap()
        {
            var url = string.Format("http://astrum.amebagames.com/_/extramap?page=1&size=4");
            var result = _client.GetXHR(url);
            _client.Access("extramap");

            return JsonConvert.DeserializeObject<MapInfo>(result);
        }

        protected override StageInfo EnterStage()
        {
            MapInfo maps = ExtraMap();
            AreaInfo area = maps.list.Find(a => (a.isNew || a.order == 1) && a.stock > 0);

            var areaId = area._id;
            if (area.status == 1)
            {
                var values = new Dictionary<string, object>
                {
                   { "areaId", areaId },
                   { "zoneId", "special_chapter1" }
                };
                _client.PostXHR("http://astrum.amebagames.com/_/stage/open", values);
            }
            
            var url = string.Format("http://astrum.amebagames.com/_/stage?areaId={0}",areaId);
            var result = _client.GetXHR(url);

            var stage = JsonConvert.DeserializeObject<StageInfo>(result);

            InfoPrinter.PrintStageInfo(stage, _client.ViewModel);
            InfoUpdater.UpdateStageView(stage.initial, _client.ViewModel);

            _client.DelayShort();
            return stage;
        }
    }
}

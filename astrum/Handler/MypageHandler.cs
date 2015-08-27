using Astrum.Http;
using Astrum.Json.Event;
using Astrum.Json.Mypage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    class MypageHandler
    {
        private AstrumClient _client;

        public MypageHandler(AstrumClient client)
        {
            _client = client;
        }
        
        public void Run()
        {
            var responseString = _client.GetXHR("http://astrum.amebagames.com/_/mypage");
            var mypage = JsonConvert.DeserializeObject<MypageInfo>(responseString);

            InfoPrinter.PrintMypage(mypage, _client.ViewModel);
            InfoUpdater.UpdateMypageView(mypage, _client.ViewModel);

            _client.Access("mypage");

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
            _client.GetXHR("http://astrum.amebagames.com/_/loginbonus");
        }

        private void LoginBonusEvent()
        {
            _client.GetXHR("http://astrum.amebagames.com/_/loginbonus/event");
        }

        private void LoginBonusLongLogin()
        {
            _client.GetXHR("http://astrum.amebagames.com/_/loginbonus/long");
        }

        public void Profile()
        {
            var result = _client.GetXHR("http://astrum.amebagames.com/_/profile");
            var profile = JsonConvert.DeserializeObject<ProfileInfo>(result);

            if (_client.DownloadUserAvatar(profile.user.leader))
            {
                _client.ViewModel.Leader = profile.user.leader.md5.sd;
            }
        }

        public void EventStatus()
        {
            var responseString = _client.GetXHR("http://astrum.amebagames.com/_/event/status");
            var eventStatus = JsonConvert.DeserializeObject<EventStatus>(responseString);

            foreach (var @event in eventStatus.list.Where(ev => ev.status))
            {
                switch (@event.type)
                {
                    case "furyraid":
                        _client.ViewModel.IsFuryRaidEnable = true;
                        _client.ViewModel.IsLimitedRaidEnable = false;
                        _client.ViewModel.IsBreedingEnable = false;
                        _client.ViewModel.FuryRaidEventId = @event._id;
                        break;
                    case "limitedraid":
                        _client.ViewModel.IsFuryRaidEnable = false;
                        _client.ViewModel.IsLimitedRaidEnable = true;
                        _client.ViewModel.IsBreedingEnable = false;
                        _client.ViewModel.LimitedRaidEventId = @event._id;
                        break;
                    case "breeding":
                        _client.ViewModel.IsFuryRaidEnable = false;
                        _client.ViewModel.IsLimitedRaidEnable = false;
                        _client.ViewModel.IsBreedingEnable = true;
                        _client.ViewModel.BreedingEventId = @event._id;
                        break;
                    case "raid":
                        break;
                }
            }

            if(_client.ViewModel.IsFuryRaidEnable)
            {
                _client.ViewModel.IsFuryRaid = true;
                _client.FuryRaid();
            }
            else if (_client.ViewModel.IsLimitedRaidEnable)
            {
                _client.ViewModel.IsLimitedRaid = true;
                _client.LimitedRaid();
            }
            else if(_client.ViewModel.IsBreedingEnable)
            {
                //
            }

            if (!_client.ViewModel.Fever)
            {
                _client.ViewModel.IsFuryRaid = false;
                _client.ViewModel.IsLimitedRaid = false;
                _client.ViewModel.IsBreedingRaid = false;
                _client.Raid();
            }
        }
    }
}

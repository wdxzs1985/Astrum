using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Json
{
    public class LoginUserList
    {
        public List<LoginUser> list { get; set; }
    }

    public class LoginUser
    {
        public string username { get; set; }

        public string password { get; set; }

        public string name { get; set; }

        public int minstaminastock { get; set; }

        public int minbpstock { get; set; }

        public int keepstamina { get; set; }

        public string leader { get; set; }
    }
}

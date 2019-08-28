using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTest
{
    public class UserXp
    {
        public string DiscordID;
        public string OsrsName;
        public string OsrsXP;
        public DateTime LastUpdate;

        public UserXp(string DiscordID, string OsrsName, string OsrsXP, DateTime LastUpdate)
        {
            this.DiscordID = DiscordID;
            this.OsrsName = OsrsName;
            this.OsrsXP = OsrsXP;
            this.LastUpdate = LastUpdate;
        }
    }
}

using JzXpWasteGraph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GraphTest
{
    class UserXp
    {
        public string DiscordID;
        public DateTime LastUpdate;
        public PlayerInfo Player;
    }

    class FileHandler
    {
        public static List<long> XP = new List<long>();
        public static List<long> XPDiff = new List<long>();
        public static List<DateTime> Date = new List<DateTime>();
        public static List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
        public static int TotalDaysWasted { get; set; }
        public static int LongestStreak { get; set; }

        public static void LoadFile()
        {
            List<UserXp> XpList = new List<UserXp>();
            if (File.Exists(@"File"))
            {
                var lines = File.ReadLines(@"File");
                //deserialize each line and add to list
                foreach (var line in lines)
                {
                    XpList.Add(JsonConvert.DeserializeObject<UserXp>(line));
                }
            }
            DateTime date = XpList.First().LastUpdate;
            long LastXp = XpList.First().Player.Overall.Experience;
            PlayerInfo LastPlayerInfo = XpList.First().Player;
            DateTime LastDate = date;
            int i = 0;
            int CurrentStreak = 0;
            //fills in the blanks where there is no data for the day which presumes he has not gained xp for that day.
            while (i != XpList.Count)
            {
                //handle a double entry - changes the last entry to be the highest xp on that day
                if (LastDate.Month == XpList[i].LastUpdate.Month && LastDate.Day == XpList[i].LastUpdate.Day && i != 0)
                {
                    long prevXPDeff = XPDiff.Last();
                    XPDiff.RemoveAt(XPDiff.Count - 1);
                    XP.RemoveAt(XP.Count - 1);
                    PlayerInfos.RemoveAt(PlayerInfos.Count - 1);
                    long NewXp = XpList[i].Player.Overall.Experience;
                    XPDiff.Add((NewXp-LastXp) + prevXPDeff);
                    LastXp = NewXp;
                    XP.Add(LastXp);
                    LastPlayerInfo = XpList[i].Player;
                    PlayerInfos.Add(LastPlayerInfo);
                    i++;
                }
                else
                {
                    long XpDiff = 0;
                    //if the date and month of the incrementing date match the current data selection add in the new xp and increment i.
                    if (date.Month == XpList[i].LastUpdate.Month && date.Day == XpList[i].LastUpdate.Day)
                    {
                        long NewXp = XpList[i].Player.Overall.Experience;
                        XpDiff = NewXp - LastXp;
                        LastXp = NewXp;
                        LastPlayerInfo = XpList[i].Player;
                        i++;
                        CurrentStreak = 0;
                    }
                    if (XpDiff == 0)
                    {
                        CurrentStreak++;
                        LongestStreak = CurrentStreak > LongestStreak ? CurrentStreak : LongestStreak;  
                        TotalDaysWasted++;
                    }
                    //add values to the lists
                    LastDate = date;
                    XP.Add(LastXp);
                    XPDiff.Add(XpDiff);
                    Date.Add(date);
                    PlayerInfos.Add(LastPlayerInfo);
                    //add one more day onto the incrementer if it isnt the last i so that the next while loop will add a day
                    date = i == XpList.Count ? date : date.AddDays(1);
                }
            }
            //for days between last update and now
            while (!(date.Month == DateTime.Now.Month && date.Day == DateTime.Now.Day))
            {
                //handle skipping the check due to double entry on day thing -> this will break if it goes over the year LUL.
                if(date.Month > DateTime.Now.Month || (date.Month == DateTime.Now.Month && date.Day > DateTime.Now.Day))
                {
                    break;
                }
                //add one more day onto the incrementer
                date = date.AddDays(1);
                XP.Add(LastXp);
                XPDiff.Add(0);
                Date.Add(date);
                PlayerInfos.Add(LastPlayerInfo);
                CurrentStreak++;
                LongestStreak = CurrentStreak > LongestStreak ? CurrentStreak : LongestStreak;
                TotalDaysWasted++;
            }
        }

        public static List<long> GetSkillList(List<PlayerInfo> PlayerInfos, string SkillName, string Property)
        {
            List<long> SkillList = new List<long>();
            for (int i = 0; i < PlayerInfos.Count; i++)
            {
                PlayerInfo player = PlayerInfos[i];
                Type playerinfo = typeof(PlayerInfo);
                Skill skill = (Skill)playerinfo.GetProperty(SkillName).GetValue(player, null);
                SkillList.Add(skill.Experience);
            }
            return SkillList;   
        }
    }
}

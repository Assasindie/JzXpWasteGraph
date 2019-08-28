using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphTest
{
    class FileHandler
    {
        public static LinkedList<long> XP = new LinkedList<long>();
        public static LinkedList<long> XPDiff = new LinkedList<long>();
        public static LinkedList<string> Date = new LinkedList<string>();

        public static void LoadFile()
        {
            List<UserXp> XpList = new List<UserXp>();
            if (File.Exists(@"location"))
            {
                var lines = File.ReadLines(@"location");
                //deserialize each line and add to list
                foreach (var line in lines)
                {
                    XpList.Add(JsonConvert.DeserializeObject<UserXp>(line));
                }
            }
            DateTime date = XpList.First().LastUpdate;
            long LastXp = int.Parse(XpList.First().OsrsXP);
            DateTime LastDate = date;
            int i = 0;
            //fills in the blanks where there is no data for the day which presumes he has not gained xp for that day.
            while (i != XpList.Count)
            {
                //handle a double entry - changes the last entry to be the highest xp on that day
                if (LastDate.Month == XpList[i].LastUpdate.Month && LastDate.Day == XpList[i].LastUpdate.Day && i != 0)
                {
                    long prevXPDeff = XPDiff.Last();
                    XPDiff.RemoveLast();
                    XP.RemoveLast();
                    long NewXp = int.Parse(XpList[i].OsrsXP);
                    XPDiff.AddLast((NewXp-LastXp) + prevXPDeff);
                    LastXp = NewXp;
                    XP.AddLast(LastXp);
                    i++;
                }
                else
                {
                    long XpDiff = 0;
                    //if the date and month of the incrementing date match the current data selection add in the new xp and increment i.
                    if (date.Month == XpList[i].LastUpdate.Month && date.Day == XpList[i].LastUpdate.Day)
                    {
                        long NewXp = int.Parse(XpList[i].OsrsXP);
                        XpDiff = NewXp - LastXp;
                        LastXp = NewXp;
                        i++;
                    }
                    //add values to the lists
                    LastDate = date;
                    XP.AddLast(LastXp);
                    XPDiff.AddLast(XpDiff);
                    Date.AddLast(date.Day + "/" + date.Month);
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
                XP.AddLast(LastXp);
                XPDiff.AddLast(0);
                Date.AddLast(date.Day + "/" + date.Month);
            }
        }
    }
}

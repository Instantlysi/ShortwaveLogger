using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SWLogger
{
    class Schedule
    {
        public string Frequency { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }
        public string BroadcastTime { get; }
        public string Station { get; }
        public string Country { get; }
        public string Language { get; }
        public string Days { get; }
        
        public Schedule()
        {
            Frequency = "";
            StartTime = new TimeSpan(0,0,0);
            EndTime = new TimeSpan(0, 0, 0);
            Station  = "";
            Country = "";
            Language = "";
            Days = "";
            BroadcastTime = "";
        }

        public Schedule(string[] entry)
        {

            string[] timing = entry[1].Split('-');
            timing[0] = timing[0].Insert(2, ":");
            timing[1] = timing[1].Insert(2, ":");
            if (timing[1] == "24:00")
            {
                timing[1] = "23:59";
            }
            StartTime = TimeSpan.Parse(timing[0]);
            EndTime = TimeSpan.Parse(timing[1]);
            BroadcastTime = entry[1];
            Frequency = entry[0];
            Station = entry[4];
            Country = entry[3];
            Language = entry[5];
            Days = entry[2];            
        }
    }
}

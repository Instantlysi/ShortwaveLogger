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
        public string BroadcastTime { get; }
        public string Station { get; }
        public string Country { get; }
        public string Language { get; }
        public string Days { get; }
               
        public Schedule()
        {
            Frequency = "";
            BroadcastTime = "";
            Station  = "";
            Country = "";
            Language = "";
            Days = "";
        }

        public Schedule(string[] entry)
        {
            Frequency = entry[0];
            BroadcastTime = entry[1];
            Station = entry[4];
            Country = entry[3];
            Language = entry[5];
            Days = entry[2];            
        }
    }
}

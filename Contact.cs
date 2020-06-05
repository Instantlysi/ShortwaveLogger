using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWLogger
{
    class Contact
    {
        public string Date { get; }
        public string Frequency { get; }
        public string Station { get; }
        public string Country { get; }
        public string Language { get; }
        public string TimeHeard { get; }
        public string Broadcast { get; }
        public string Notes { get; }
        public bool? WebSDR { get; } = false;
        public Contact()
        {
            Date = "";
            Frequency = "";
            Station = "";
            Country = "";
            Language = "";
            TimeHeard = "";
            Broadcast = "";
            Notes = "";
            WebSDR = false;
        }
        public Contact(string[] data, bool? websdr, DateTime dateHeard)
        {
            Frequency = data[0];
            Station = data[1];
            Country = data[2];
            Language = data[3];
            Broadcast = data[4];
            Notes = data[5];
            TimeHeard = data[6];
            WebSDR = websdr;
            Date = dateHeard.Date.ToString("d");
        }
        public Contact(string[] data, bool? websdr, DateTime dateHeard, string mode)
        {
            Frequency = data[0];
            Station = data[1];
            Country = data[2];
            Language = data[3];
            Broadcast = data[5];
            Notes = data[6];
            TimeHeard = data[4];
            WebSDR = websdr;
            Date = dateHeard.Date.ToString("d");
        }
    }
}

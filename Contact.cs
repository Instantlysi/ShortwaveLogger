using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWLogger
{
    class Contact
    {
        public string Frequency { get; }
        public string Station { get; }
        public string Country { get; }
        public string Language { get; }
        public string TimeHeard { get; }
        public string Broadcast { get; }
        public string Notes { get; }
        public bool? WebSDR { get; } = false;

        public bool QuickAdd;
        public Contact()
        {
            Frequency = "";
            Station = "";
            Country = "";
            Language = "";
            TimeHeard = "";
            Broadcast = "";
            Notes = "";
            WebSDR = false;
        }
        public Contact(string[] data, bool? websdr)
        {
            Frequency = data[0];
            Station = data[1];
            Country = data[2];
            Language = data[3];
            Broadcast = data[4];
            Notes = data[5];
            TimeHeard = data[6];
            WebSDR = websdr;
        }
        public Contact(string[] data, bool? websdr, bool quick)
        {

        }
    }
}

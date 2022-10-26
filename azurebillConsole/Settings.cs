using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azurebillConsole
{
    public class Settings
    {
        public int siteCount { get; set; }
        public int folderCount { get; set; }
        public int fileCount { get; set; }
        public int NumberOfUpdates { get; set; }
        public int maxtrycount { get; set; }
        public int trypause { get; set; }
        public string fulllog { get; set; }
        public string errorlog { get; set; }
        public string runmode { get; set; }
        public string saveItem { get; set; }
        public string saveItemTest1 { get; set; }
        public string sitedirectlink { get; set; }
    }
}

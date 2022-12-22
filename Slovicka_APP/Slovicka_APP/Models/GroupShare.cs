using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class GroupShare
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string FirstLang { get; set; }
        public string SecondLang { get; set; }
        public string Translates { get; set; }
    }
}

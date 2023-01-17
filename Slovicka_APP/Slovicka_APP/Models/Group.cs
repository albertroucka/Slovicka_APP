using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class Group
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string FirstLang { get; set; }
        public string SecondLang { get; set; }
        public int NumberOfExercises { get; set; }
        public int SuccessRate { get; set; }
    }
}

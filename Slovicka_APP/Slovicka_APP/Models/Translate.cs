using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class Translate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FirstWord { get; set; }
        public string SecondWord { get; set; }
        public string GroupName { get; set; }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public int NumberOfTrophies { get; set; }
        public string AllGroups { get; set; }
    }
}

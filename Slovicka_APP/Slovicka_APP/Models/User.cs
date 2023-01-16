using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slovicka_APP.Models
{
    public class LocalUser : FirebaseUser
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }

    public class FirebaseUser
    {
        public string FirebaseId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int NumberOfTrophies { get; set; }
        public int NumberOfExercises { get; set; }
        public int NumberOfCreatedGroups { get; set; }
        public int NumberOfSharedGroups { get; set; }
        public string AllGroups { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}

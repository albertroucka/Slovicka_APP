using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Slovicka_APP.Models
{
    class MainClass
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public string GetUserName()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<User>();
                var users = conn.Table<User>().ToList();

                if (ff.GetFirestoreUserAuth())
                {
                    return ff.GetFirestoreUserName();
                }
                else if (users.Count > 0)
                {
                    return users[0].UserName;
                }
                else
                {
                    return "Nepřihlášen";
                }
            }
        }

        public string GetUserTrophiesCount()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<User>();
                var users = conn.Table<User>().ToList();

                if (ff.GetFirestoreUserAuth())
                {
                    return ff.GetFirestoreUserTrophies();
                }
                else if (users.Count > 0)
                {
                    return users[0].NumberOfTrophies.ToString();
                }
                else
                {
                    return "???";
                }
            }
        }

        public User GetUser()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<User>();
                var users = conn.Table<User>().ToList();

                if (users.Count > 0)
                {
                    return users[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void AnswerColor(bool result)
        {
            if (result)
            {
                App.Current.Resources["app_color_answer"] = Color.FromHex("#04AD33");
                await Task.Delay(500);
                App.Current.Resources["app_color_answer"] = Color.FromHex("#FFFFFF");
            }
            else
            {
                App.Current.Resources["app_color_answer"] = Color.FromHex("#F80606");
                await Task.Delay(500);
                App.Current.Resources["app_color_answer"] = Color.FromHex("#FFFFFF");
            }
        }

        public bool UpdateUserStats(int trophiesCount)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                User user = GetUser();

                if (user != null)
                {
                    user.NumberOfTrophies = user.NumberOfTrophies + trophiesCount;
                    conn.CreateTable<User>();
                    int rows = conn.Update(user);
                    if (rows > 0)
                    {
                        return false;
                    }
                    else
                    {
                        
                    }
                }
                return true;
            }
        }

        public List<string> GetAllExerciseTypes()
        {
            List<string> allExercises = new List<string>() { "Překlad slov", "Výběr z možností", "Kartičky" };
            return allExercises;
        }

        public List<string> GetAllLanguages()
        {
            List<string> Lang = new List<string>() { "Anglicky", "Česky", "Francouzsky", "Italsky", "Německy", "Polsky", "Slovensky", "Španělsky" };
            return Lang;
        }
    }
}

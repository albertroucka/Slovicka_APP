using Plugin.CloudFirestore;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Slovicka_APP.Models
{
    public class FirebaseFirestore
    {
        User firestoreUser = new User(); bool firestoreUserAuth = false;

        public async void FirebaseA() 
        {
            User user = new User() { UserName = "Admin123", Password = "admin123", NumberOfTrophies = 1025, AllGroups = "ALL" };
            await CrossCloudFirestore.Current
                         .Instance
                         .Collection("users")
                         .AddAsync(user);
        }

        public async Task<bool> FirebaseB(string doc)
        {
            var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("groups")
                                        .Document(doc)
                                        .GetAsync();

            var result = document.ToObject<User>();
            return true;
        }

        public async Task GetFirestoreUserData()
        {
            try
            {
                var query = await CrossCloudFirestore.Current
                                     .Instance
                                     .Collection("users")
                                     .WhereEqualsTo("UserName", "Albert R.")
                                     .GetAsync().ConfigureAwait(false);

                var result = query.ToObjects<User>();
                firestoreUserAuth = true;
            }
            catch (Exception)
            {
                firestoreUserAuth = false;
            }
        }

        public bool GetFirestoreUserAuth()
        {
            return this.firestoreUserAuth;
        }

        public string GetFirestoreUserName()
        {
            return this.firestoreUser.UserName;
        }

        public string GetFirestoreUserTrophies()
        {
            return firestoreUser.NumberOfTrophies.ToString();
        }


        //Sdílení skupin
        //Ověřovní, nahrávání nebo aktualizování skupin v FF

        public void InsertNewGroup(GroupShare data)
        {
            Group newGroup = new Group()
            {
                GroupName = data.GroupName,
                FirstLang = data.FirstLang,
                SecondLang = data.SecondLang,
                NumberOfExercises = 0,
                SuccessRate = 100
            };

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>(); int i = 0;
                var groups = conn.Table<Group>().ToList();

                while (i < groups.Count)
                {
                    var quest = groups[i];
                    if (quest.GroupName == newGroup.GroupName)
                    {
                        while (quest.GroupName == newGroup.GroupName)
                        {
                            newGroup.GroupName = newGroup.GroupName + "x";
                        }
                    }
                    i++;
                }

                int rows = conn.Insert(newGroup);
                if (rows > 0) { }
            }

            InsertNewTranslates(data, newGroup.GroupName);
        }

        public void InsertNewTranslates(GroupShare data, string groupName)
        {
            string allTranslates = data.Translates;
            int index = allTranslates.IndexOf("+"); string str;
            List<string> translates = new List<string>();
            while (index > 0)
            {
                str = allTranslates.Substring(0, index - 1);
                allTranslates = allTranslates.Remove(0, index + 2);
                translates.Add(str);
                index = allTranslates.IndexOf("+");
            }
            translates.Add(allTranslates);

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>();

                foreach (var item in translates)
                {
                    int i = item.IndexOf("-");
                    string firstLang = item.Substring(0, i);
                    string secondLang = item.Remove(0, i + 1);

                    Translate newTranslate = new Translate()
                    {
                        GroupName = groupName,
                        FirstWord = firstLang,
                        SecondWord = secondLang
                    };

                    int rows = conn.Insert(newTranslate);
                    if (rows > 0) { }

                }
            }
        }

        public string GenereteGroupShareCode()
        {
            Random r = new Random(); string groupCode = string.Empty;
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int i = 0; i < 6; i++)
            {
                int type = r.Next(2);
                if (type == 0)
                {
                    groupCode += r.Next(0, 9);
                }
                else
                {
                    int i0 = alphabet.Length;
                    groupCode += alphabet[r.Next(0, 25)];
                }
            }

            return groupCode;
        }
    }
}

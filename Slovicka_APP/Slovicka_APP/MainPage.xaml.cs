using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Slovicka_APP
{
    public partial class MainPage : ContentPage
    {
        MainClass mainClass = new MainClass();
        FirebaseFirestore ff = new FirebaseFirestore();

        public MainPage()
        {
            InitializeComponent();
            SetValues();
        }

        public void SetValues()
        {
            btn_account.Text = mainClass.GetUserName();
            btn_trophies.Text = mainClass.GetUserTrophiesCount();
        }

        private void btn_practise_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Options());
        }

        private void btn_wordsAdd_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WordsAdd());
        }

        private void btn_groupsAdd_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GroupsAdd());
        }

        private void btn_account_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<User>();
                var users = conn.Table<User>().ToList();

                if (users.Count == 0)
                {
                    User user = new User() { UserName = "Albert R.", Password = "admin", NumberOfTrophies = 0, AllGroups = null };

                    int rows = conn.Insert(user);
                    if (rows > 0)
                    {
                        DisplayAlert("Úspěch", "Uživatel byl úspěšně vytvořen!", "Ok");

                    }
                    else
                    {
                        DisplayAlert("Chyba", "Uživatele se nepodařilo vytvořit!", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("Chyba", "Uživatel je již vytvořen!", "Ok");
                }
            }

            btn_account.Text = mainClass.GetUserName();
            btn_trophies.Text = mainClass.GetUserTrophiesCount();
        }
    }
}

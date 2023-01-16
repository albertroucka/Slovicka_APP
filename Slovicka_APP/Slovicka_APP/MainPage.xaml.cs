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
            CreateUserOnStart();
            ff.DownloadFirebaseUserData();
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
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count > 1)
                {
                    Navigation.PushAsync(new UserDetail());
                }
                else
                {
                    Navigation.PushAsync(new UserLogin());
                }
            }
        }

        private void CreateUserOnStart()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count == 0)
                {
                    LocalUser user = new LocalUser() { FirebaseId = null, UserName = "Nepřihlášen", UserEmail = null, NumberOfTrophies = 0, NumberOfExercises = 0, NumberOfCreatedGroups = 0, NumberOfSharedGroups = 0, AllGroups = null, RegistrationDate = DateTime.Now };

                    int rows = conn.Insert(user);
                    if (rows > 0)
                    { 
                    
                    }                       
                }
            }

            btn_account.Text = mainClass.GetUserName();
            btn_trophies.Text = mainClass.GetUserTrophiesCount();
        }

        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return true;
        }
    }
}

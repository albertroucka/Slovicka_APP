using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetail : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore(); MainClass mainClass = new MainClass();

        public UserDetail()
        {
            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();
                LocalUser user = users[1];

                lb_name.Text = "Uživ. jméno: " + user.UserName;
                lb_email.Text = "E-mail: " + user.UserEmail;
                lb_numberOfTrophies.Text = "Získané poháry: " + user.NumberOfTrophies.ToString();
                lb_numberOfExercises.Text = "Počet procvičení: " + user.NumberOfExercises.ToString();
                lb_numberOfCreatedGroups.Text = "Vytvořeno skupin: " + user.NumberOfCreatedGroups.ToString();
                lb_numberOfSharedGroups.Text = "Sdíleno skupin: " + user.NumberOfSharedGroups.ToString();
                lb_registrationDate.Text = "Datum registrace: " + user.RegistrationDate.ToString("dd.MM.yyyy");

                ent_name.Text = user.UserName;
            }
        }

        private void btn_logout_Clicked(object sender, EventArgs e)
        {
            FirebaseFirestore ff = new FirebaseFirestore();
            ff.FirebaseUserLogout();
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            Navigation.PushAsync(new MainPage());
            return true;
        }

        private void btn_updateUserData_Clicked(object sender, EventArgs e)
        {
            if (btn_updateUserData.Text == "Upravit uživatele")
            {
                lb_name.IsVisible = false; ent_name.IsVisible = true;
                btn_updateUserData.Text = "Uložit změny";
            }
            else
            {
                ent_name.IsVisible = false; lb_name.IsVisible = true; 
                UpdateUserData();
                btn_updateUserData.Text = "Upravit uživatele";
            }
        }

        private void UpdateUserData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                LocalUser user = mainClass.GetUser();
                user.UserName = ent_name.Text;

                if (user != null)
                {
                    conn.CreateTable<LocalUser>();
                    int rows = conn.Update(user);
                    if (rows > 0)
                    {
                        if (user.FirebaseId != null)
                        {
                            FirebaseUser firebaseUser = new FirebaseUser() { FirebaseId = user.FirebaseId, UserName = ent_name.Text, UserEmail = user.UserEmail, NumberOfTrophies = user.NumberOfTrophies, NumberOfExercises = user.NumberOfExercises, NumberOfCreatedGroups = user.NumberOfCreatedGroups, NumberOfSharedGroups = user.NumberOfSharedGroups, RegistrationDate = user.RegistrationDate, AllGroups = user.AllGroups };
                            ff.UpdateFirebaseUser(firebaseUser);
                            DisplayAlert("Úspěch", "Uživatel byl úspěšně aktualizován!", "Ok");
                        }
                    }
                }
            }
        }
    }
}
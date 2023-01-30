using Firebase.Auth;
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

                lb_name.Text = $"<b>Uživ. jméno: </b> {user.UserName}";
                lb_email.Text = $"<b>E-mail: </b>" + user.UserEmail;
                lb_numberOfTrophies.Text = $"<b>Získané poháry: </b>" + user.NumberOfTrophies.ToString();
                lb_numberOfExercises.Text = $"<b>Počet procvičení: </b>" + user.NumberOfExercises.ToString();
                lb_numberOfCreatedGroups.Text = $"<b>Vytvořeno skupin: </b>" + user.NumberOfCreatedGroups.ToString();
                lb_numberOfSharedGroups.Text = $"<b>Sdíleno skupin: </b>" + user.NumberOfSharedGroups.ToString();
                lb_registrationDate.Text = $"<b>Datum registrace: </b>" + user.RegistrationDate.ToString("dd.MM.yyyy");

                ent_name.Text = user.UserName;
            }
        }

        private void btn_logout_Clicked(object sender, EventArgs e)
        {
            ai_loading.IsVisible = true;
            FirebaseFirestore ff = new FirebaseFirestore();
            ff.FirebaseUserLogout(ai_loading);
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
                string edit = ent_name.Text; lb_name.Text = $"<b>Uživ. jméno: </b> {edit}";
                UpdateUserData(edit);
                btn_updateUserData.Text = "Upravit uživatele";
            }
        }

        private void ent_name_Completed(object sender, EventArgs e)
        {
            ent_name.IsVisible = false; lb_name.IsVisible = true;
            string edit = ent_name.Text; lb_name.Text = $"<b>Uživ. jméno: </b> {edit}";
            UpdateUserData(edit);
            btn_updateUserData.Text = "Upravit uživatele";
        }

        private void UpdateUserData(string edit)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                LocalUser user = mainClass.GetUser();
                user.UserName = edit;

                if (user != null)
                {
                    conn.CreateTable<LocalUser>();
                    int rows = conn.Update(user);
                    if (rows > 0)
                    {
                        if (user.FirebaseId != null)
                        {
                            ai_loading.IsVisible = true;
                            FirebaseUser firebaseUser = new FirebaseUser() { FirebaseId = user.FirebaseId, UserName = edit, UserEmail = user.UserEmail, NumberOfTrophies = user.NumberOfTrophies, NumberOfExercises = user.NumberOfExercises, NumberOfCreatedGroups = user.NumberOfCreatedGroups, NumberOfSharedGroups = user.NumberOfSharedGroups, RegistrationDate = user.RegistrationDate, AllGroups = user.AllGroups };
                            ff.UpdateFirebaseUser(firebaseUser, ai_loading);
                            DisplayAlert("Úspěch", "Uživatel byl úspěšně aktualizován!", "Ok");
                        }
                    }
                }
            }
        }
    }
}
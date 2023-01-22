using Slovicka_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserLogin : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public UserLogin()
        {
            InitializeComponent();
        }

        private void btn_confirm_Clicked(object sender, EventArgs e)
        {
            LogInUser();
        }

        private void lb_forgottenpass_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserForgottenPass());
        }

        private void lb_register_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserRegistration());
        }

        private void ent_password_Completed(object sender, EventArgs e)
        {
            LogInUser();
        }

        private void LogInUser()
        {
            ai_loading.IsVisible = true;
            ff.FirebaseLogIn(ent_username.Text, ent_password.Text, ai_loading);
        }
    }
}
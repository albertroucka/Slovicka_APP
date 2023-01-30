using Firebase.Auth;
using Java.Security;
using Org.Apache.Http.Authentication;
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
    public partial class UserVerification : ContentPage
    {
        FirebaseAuthProvider authProvider; FirebaseAuthLink auth; FirebaseFirestore ff = new FirebaseFirestore(); string pass;

        public UserVerification(FirebaseAuthProvider authProvider, FirebaseAuthLink auth, string password)
        {
            InitializeComponent();
            this.authProvider = authProvider;
            var send = authProvider.SendEmailVerificationAsync(auth.FirebaseToken);
            auth = authProvider.SignInWithEmailAndPasswordAsync(auth.User.Email, password).Result;
            this.auth = auth; this.pass = password;
            ff.FirebaseUserLogout(null);
        }

        private async void btn_confirm_Clicked(object sender, EventArgs e)
        {
            await auth.RefreshUserDetails();
            bool verify = auth.User.IsEmailVerified;
            await authProvider.RefreshAuthAsync(auth);
            //ff.FirebaseUserLogout(null);

            if (verify)
            {
                ff.FirebaseLogIn(auth.User.Email, pass, ai_loading);
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Chyba", "Uživatelský účet zatím nebyl ověřen, zkontrolujte svou e-mailovou schránku!", "Ok");
            }
        }
    }
}
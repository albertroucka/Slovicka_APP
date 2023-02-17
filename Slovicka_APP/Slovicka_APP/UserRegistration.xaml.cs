using Java.Net;
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
    public partial class UserRegistration : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public UserRegistration()
        {
            InitializeComponent();
            lb_conditions.Text = "*Registrací souhlasíte s podmínkami <a href='https://aplikaceslovicka.cz/privacypolicy.html'>aplikace</a>";
        }

        [Obsolete]
        private void lb_conditions_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://aplikaceslovicka.cz/privacypolicy.html"));
        }

        private void btn_confirm_Clicked(object sender, EventArgs e)
        {
            RegisterCheck();
        }

        private void ent_passwordcheck_Completed(object sender, EventArgs e)
        {
            RegisterCheck();
        }

        private void RegisterCheck()
        {
            if (ent_username.Text != null && ent_email.Text != null && ent_emailcheck.Text != null && ent_password.Text != null && ent_passwordcheck.Text != null)
            {
                if (ent_email.Text == ent_emailcheck.Text)
                {
                    if (ent_password.Text == ent_passwordcheck.Text)
                    {
                        ai_loading.IsVisible = true;
                        ff.FirebaseSignUp(ent_username.Text, ent_emailcheck.Text, ent_passwordcheck.Text, ai_loading);
                    }
                    else
                    {
                        DisplayAlert("Chyba", "Zadaná hesla se neshodují!", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("Chyba", "Zadané e-maily se neshodují!", "Ok");
                }
            }
            else
            {
                DisplayAlert("Chyba", "Pro dokončení registrace vyplňte prosím všechna povinná pole!", "Ok");
            }
        }
    }
}
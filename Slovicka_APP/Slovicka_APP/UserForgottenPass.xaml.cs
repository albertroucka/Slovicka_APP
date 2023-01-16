﻿using Slovicka_APP.Models;
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
    public partial class UserForgottenPass : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public UserForgottenPass()
        {
            InitializeComponent();
        }

        private void btn_confirm_Clicked(object sender, EventArgs e)
        {
            string email = ent_useremail.Text;
            ff.FirebaseUserResetPass(email);
        }
    }
}
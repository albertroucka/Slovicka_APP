using Android.App;
using Android.Content.PM;
using Android.OS;
using Slovicka_APP.Droid;
using Slovicka_APP.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Slovicka_APP
{
    [Activity(Label = "Slovicka_APP", Icon = "@mipmap/ic_launcher", Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]

    class SplashScreenActivity:Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
            Thread.Sleep(2000);
            Finish();
        }
    }
}

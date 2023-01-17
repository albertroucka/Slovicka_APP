using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Result : ContentPage
    {
        MainClass mainClass = new MainClass(); FirebaseFirestore ff = new FirebaseFirestore();

        public Result(int points, List<Translate> wrongAnswers, Group selectedGroup)
        {
            InitializeComponent();
            lb_score.Text = $"Skóre: {points}/10";
            double successRate = Convert.ToDouble(points) / 10 * 100;
            lb_successRate.Text = $"Úspěšnost: {successRate}%";
            SetTrophies(successRate);
            UpdateGroupStats(selectedGroup, successRate);
            if (wrongAnswers.Count == 0) { cv_answers.IsVisible = false; lb_answers.IsVisible = true; } 
            else { cv_answers.ItemsSource = wrongAnswers; }
        }

        private void SetTrophies(double successRate)
        {
            int trophiesCount;

            if (successRate > 85)
            {
                trophiesCount = 3;
                img_trophy0.Source = "trophy.png";
                img_trophy1.Source = "trophy.png";
                img_trophy2.Source = "trophy.png";
            }
            else if (successRate > 55)
            {
                trophiesCount = 2;
                img_trophy0.Source = "trophy.png";
                img_trophy1.Source = "trophy.png";
                img_trophy2.Source = "trophy_gray.png";
            }
            else if (successRate > 25)
            {
                trophiesCount = 1;
                img_trophy0.Source = "trophy.png";
                img_trophy1.Source = "trophy_gray.png";
                img_trophy2.Source = "trophy_gray.png";
            }
            else
            {
                trophiesCount = 0;
                img_trophy0.Source = "trophy_gray.png";
                img_trophy1.Source = "trophy_gray.png";
                img_trophy2.Source = "trophy_gray.png";
            }

            //přidání trofejí do uživatelského účtu
            mainClass.UpdateUserStats(trophiesCount);
        }

        private void UpdateGroupStats(Group selectedGroup, double successRate)
        {
            selectedGroup.NumberOfExercises = selectedGroup.NumberOfExercises + 1;
            double percentage = (selectedGroup.SuccessRate * (selectedGroup.NumberOfExercises-1)  + successRate) / selectedGroup.NumberOfExercises;
            selectedGroup.SuccessRate = Convert.ToInt32(percentage);

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                int rows = conn.Update(selectedGroup);
                if (rows > 0) { }
            }
            ff.UpdateFirebaseUserGroups(false);
        }

        private void btn_PlayAgin_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Options());
        }

        private async void btn_MainMenu_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new Options());
            return true;
        }
    }
}
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
    public partial class GameCards : ContentPage
    {
        QuestionCards questionCards = new QuestionCards();
        string question; string correctAnswer; List<Translate> GameList; List<Translate> CardsList = new List<Translate>(); bool translate; int i;

        public GameCards(List<Translate> gameList, bool translate, string firstLang, string secondLang)
        {
            InitializeComponent();
            this.GameList = gameList;
            this.translate = translate;
            CardsList = questionCards.GetRandomSequence(gameList);
            Start();
        }   

        private void Start()
        {
            i = 0;
            var trans = CardsList[i];
            question = trans.FirstWord;
            correctAnswer = trans.SecondWord;
            lb_groupName.Text = trans.GroupName;
            lb_count.Text = $"{i + 1}/{CardsList.Count}";
            if (translate == false)
            {
                btn_card.Text = question;
            }
            else
            {
                btn_card.Text = correctAnswer;
            }
            UpdateUserTrophies();
        }

        private void btn_card_Clicked(object sender, EventArgs e)
        {
            if (btn_card.BackgroundColor == Color.FromHex("#2196F3"))
            {
                btn_card.BackgroundColor = Color.FromHex("#FFFFFF");
                btn_card.TextColor = Color.FromHex("#2196F3");
                if (translate == false) { btn_card.Text = correctAnswer; }
                else { btn_card.Text = question; }
            }
            else
            {
                btn_card.BackgroundColor = Color.FromHex("#2196F3");
                btn_card.TextColor = Color.FromHex("#FFFFFF");
                if (translate == false) { btn_card.Text = question; }
                else { btn_card.Text = correctAnswer; }
            }
        }

        private void btn_back_Clicked(object sender, EventArgs e)
        {
            if (i - 1 >= 0)
            {
                i--;
                var trans = CardsList[i];
                question = trans.FirstWord;
                correctAnswer = trans.SecondWord;
                lb_count.Text = $"{i + 1}/{CardsList.Count}";
                if (btn_card.BackgroundColor == Color.FromHex("#2196F3")) { btn_card.Text = question;}
                else { btn_card.Text = correctAnswer; }
            }
        }

        private void btn_next_Clicked(object sender, EventArgs e)
        {
            if (i + 1 < GameList.Count)
            {
                i++;
                var trans = CardsList[i];
                question = trans.FirstWord;
                correctAnswer = trans.SecondWord;
                lb_count.Text = $"{i + 1}/{CardsList.Count}";
                if (btn_card.BackgroundColor == Color.FromHex("#2196F3")) { btn_card.Text = question; }
                else { btn_card.Text = correctAnswer; }
            }
        }

        private void UpdateUserTrophies()
        {
            MainClass mainClass = new MainClass();
            mainClass.UpdateUserStats(1);
        }
    }
}
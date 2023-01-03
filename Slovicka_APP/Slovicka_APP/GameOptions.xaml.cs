using Slovicka_APP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameOptions : ContentPage
    {
        QuestionABC questions = new QuestionABC(); List<Translate> GameList; List<Translate> WrongAnswers = new List<Translate>(); MainClass mainClass = new MainClass();
        int round = 0; int points = 0; string correctAnswer; bool translate; Group selectedGroup;

        public GameOptions(List<Translate> gameList, bool translate, string firstLang, string secondLang, Group selectedGroup)
        {
            InitializeComponent();
            this.GameList = gameList;
            this.translate = translate;
            this.selectedGroup = selectedGroup;

            if (translate == false)
            {
                lb_first_lang_text.Text = $"{firstLang}:";
            }
            else
            {
                lb_first_lang_text.Text = $"{secondLang}:";
            }

            NewQuestion();
        }

        public void NewQuestion()
        {
            round++;
            lb_round.Text = "Kolo: " + Convert.ToString(round);
            lb_points.Text = "Body: " + Convert.ToString(points);
            QuestionABC question = questions.GetNewQuestion(GameList, translate);
            lb_first_lang.Text = question.QuestionText;
            btn_A.Text = question.OptionA;
            btn_B.Text = question.OptionB;
            btn_C.Text = question.OptionC;
            this.correctAnswer = question.CorrectAnswer;
        }

        public void Questions()
        {
            if (round > 9)
            {
                Navigation.PushAsync(new Result(points, WrongAnswers, selectedGroup));
            }
            else
            {
                NewQuestion();
            }
        }

        private void btn_A_Clicked(object sender, EventArgs e)
        {
            if (btn_A.Text == correctAnswer)
            {
                mainClass.AnswerColor(true);
                points++;
            }
            else
            {
                mainClass.AnswerColor(false);
                Translate translate = new Translate() { FirstWord = lb_first_lang.Text, SecondWord = correctAnswer, GroupName = btn_A.Text };
                WrongAnswers.Add(translate);
            }
            Questions();
        }

        private void btn_B_Clicked(object sender, EventArgs e)
        {
            if (btn_B.Text == correctAnswer)
            {
                mainClass.AnswerColor(true);
                points++;
            }
            else
            {
                mainClass.AnswerColor(false);
                Translate translate = new Translate() { FirstWord = lb_first_lang.Text, SecondWord = correctAnswer, GroupName = btn_B.Text };
                WrongAnswers.Add(translate);
            }
            Questions();
        }

        private void btn_C_Clicked(object sender, EventArgs e)
        {
            if (btn_C.Text == correctAnswer)
            {
                mainClass.AnswerColor(true);
                points++;
            }
            else
            {
                mainClass.AnswerColor(false);
                Translate translate = new Translate() { FirstWord = lb_first_lang.Text, SecondWord = correctAnswer, GroupName = btn_C.Text };
                WrongAnswers.Add(translate);
            }
            Questions();
        }
    }
}
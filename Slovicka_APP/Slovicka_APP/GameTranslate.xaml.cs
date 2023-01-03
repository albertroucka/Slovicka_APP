using Slovicka_APP.Models;
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
    public partial class GameTranslate : ContentPage
    {
        Question questions = new Question(); List<Translate> GameList; List<Translate> WrongAnswers = new List<Translate>(); MainClass mainClass = new MainClass();
        int round = 1; int points = 0; string correctAnswer; bool translate; Group selectedGroup;

        public GameTranslate(List<Translate> gameList, bool translate, string firstLang, string secondLang, Group selectedGroup)
        {
            InitializeComponent();
            this.GameList = gameList;
            this.translate = translate;
            this.selectedGroup = selectedGroup;

            if (translate == false)
            {
                lb_first_lang_text.Text = $"{firstLang}:";
                lb_second_lang_text.Text = $"{secondLang}:";
            }
            else
            {
                lb_first_lang_text.Text = $"{secondLang}:";
                lb_second_lang_text.Text = $"{firstLang}:";
            }

            NewQuestion();
        }

        public void NewQuestion()
        {
            lb_round.Text = "Kolo: " + Convert.ToString(round);
            lb_points.Text = "Body: " + Convert.ToString(points);
            Question question = questions.GetNewQuestion(GameList, translate);
            lb_first_lang.Text = question.QuestionText;
            this.correctAnswer = question.CorrectAnswer;
        }

        public void CheckAnswer()
        {
            string answer = ent_second_lang.Text;
            ent_second_lang.Text = "";

            if (answer != null)
            {
                if (answer.ToLower() == correctAnswer.ToLower())
                {
                    mainClass.AnswerColor(true);
                    points++;
                    lb_points.Text = Convert.ToString(points);
                }
                else
                {
                    mainClass.AnswerColor(false);
                    Translate translate = new Translate() { FirstWord = lb_first_lang.Text, SecondWord = correctAnswer, GroupName = answer };
                    WrongAnswers.Add(translate);
                }
            }
            else
            {
                mainClass.AnswerColor(false);
                Translate translate = new Translate() { FirstWord = lb_first_lang.Text, SecondWord = correctAnswer, GroupName = answer };
                WrongAnswers.Add(translate);
            }
        }

        private void btn_confirm_Clicked(object sender, EventArgs e)
        {
            CheckAnswer();

            if (round > 9)
            {
                Navigation.PushAsync(new Result(points, WrongAnswers, selectedGroup));
            }
            else
            {
                round++;
                lb_round.Text = Convert.ToString(round);
                NewQuestion();
            }
        }
    }
}
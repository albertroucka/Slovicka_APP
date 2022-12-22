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
    public partial class Options : ContentPage
    {
        MainClass mainClass = new MainClass();
        string firstLang; string secondLang;

        public Options()
        {
            InitializeComponent();
            pk_exercise.ItemsSource = mainClass.GetAllExerciseTypes();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>();
                var translates = conn.Table<Translate>().ToList();
                pk_groups.ItemsSource = translates;

                conn.CreateTable<Group>();
                var groups = conn.Table<Group>().ToList();
                pk_groups.ItemsSource = groups;
            }
        }

        private void btn_Start_Clicked(object sender, EventArgs e)
        {
            if (pk_groups.SelectedItem != null)
            {
                if (pk_exercise.SelectedItem != null)
                {
                    List<Translate> GameList = new List<Translate>();

                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.CreateTable<Translate>(); int i = 0;
                        var posts = conn.Table<Translate>().ToList();

                        int index = pk_groups.SelectedIndex;
                        string groupname = pk_groups.Items[index];

                        while (i < posts.Count)
                        {
                            var quest = posts[i];
                            if (quest.GroupName == groupname)
                            {
                                GameList.Add(posts[i]);
                            }
                            i++;
                        }

                        conn.CreateTable<Group>();
                        var groups = conn.Table<Group>().ToList();
                        var lang = groups[index];
                        firstLang = lang.FirstLang;
                        secondLang = lang.SecondLang;
                    }

                    bool translate = false;
                    if (sw_translate.IsToggled)
                    {
                        translate = true;
                    }

                    int ind = pk_exercise.SelectedIndex;
                    string exercise = pk_exercise.Items[ind];

                    if (exercise == "Výběr z možností")
                    {
                        if (GameList.Count > 2)
                        {
                            Navigation.PushAsync(new GameOptions(GameList, translate, firstLang, secondLang, pk_groups.SelectedItem as Group));
                        }
                        else
                        {
                            DisplayAlert("Chyba", "V seznamu musíte mít alespoň 3 slovíčka!", "Ok");
                        }
                    }
                    else if (exercise == "Kartičky")
                    {
                        if (GameList.Count > 0)
                        {
                            Navigation.PushAsync(new GameCards(GameList, translate, firstLang, secondLang));
                        }
                        else
                        {
                            DisplayAlert("Chyba", "V seznamu nemáte žádná slovíčka k procvičení!", "Ok");
                        }
                    }
                    else
                    {
                        if (GameList.Count > 0)
                        {
                            Navigation.PushAsync(new GameTranslate(GameList, translate, firstLang, secondLang, pk_groups.SelectedItem as Group));
                        }
                        else
                        {
                            DisplayAlert("Chyba", "V seznamu nemáte žádná slovíčka k procvičení!", "Ok");
                        }
                    }
                }
                else
                {
                    DisplayAlert("Chyba", "Musíte vybrat typ procvičení!", "Ok");
                }
            }
            else
            {
                DisplayAlert("Chyba", "Musíte vybrat skupinu!", "Ok");
            }
        }
    }
}
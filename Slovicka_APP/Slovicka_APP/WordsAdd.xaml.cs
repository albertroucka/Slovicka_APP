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
    public partial class WordsAdd : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public WordsAdd()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                var groups = conn.Table<Group>().ToList();
                pk_group.ItemsSource = groups;
                List<Group> sorted = groups.OrderBy(Group => Group.GroupName).ToList();
                lv_groups.ItemsSource = sorted;
            }
        }

        private void btn_save_Clicked(object sender, EventArgs e)
        {
            if (pk_group.SelectedItem != null)
            {
                int i = pk_group.SelectedIndex;
                string groupname = pk_group.Items[i];
                Translate translate = new Translate() { GroupName = groupname, FirstWord = firstWord.Text, SecondWord = secondWord.Text };

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Translate>();
                    int rows = conn.Insert(translate);
                    if (rows > 0)
                    {
                        ff.UpdateFirebaseUserGroups(false);
                        DisplayAlert("Úspěch", "Překlad byl úspěšně přidán!", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Chyba", "Překlad se nepovedlo přidat!", "Ok");
                    }
                }

                firstWord.Text = string.Empty;
                secondWord.Text = string.Empty;
            }
            else
            {
                DisplayAlert("Chyba", "Musíte vybrat skupinu!", "Ok");
            }

        }

        private void lv_groups_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedGroup = lv_groups.SelectedItem as Group;
            if (selectedGroup != null)
            {
                Navigation.PushAsync(new WordsGroup(selectedGroup));
            }
        }
    }
}
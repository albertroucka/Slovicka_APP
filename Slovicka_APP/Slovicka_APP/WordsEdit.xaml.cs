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
    public partial class WordsEdit : ContentPage
    {
        Group selectedGroup; Translate selectedTranslate; FirebaseFirestore ff = new FirebaseFirestore(); MainClass mainClass = new MainClass();

        public WordsEdit(Group selectedGroup, Translate selectedTranslate)
        {
            InitializeComponent();
            this.selectedGroup = selectedGroup;
            this.selectedTranslate = selectedTranslate;
            ent_firstWord.Text = selectedTranslate.FirstWord;
            ent_secondWord.Text = selectedTranslate.SecondWord;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                var groups = conn.Table<Group>().ToList();
                pk_group.ItemsSource = groups;
                pk_group.Title = selectedGroup.GroupName;
            }
        }

        private void btn_update_Clicked(object sender, EventArgs e)
        {
            if (ent_firstWord.Text == null || ent_secondWord.Text == null)
            {
                DisplayAlert("Chyba", "Pole slova nebo překladu je prázdné!", "Ok");
            }
            else if (mainClass.CheckForbiddenChars(ent_firstWord.Text) || mainClass.CheckForbiddenChars(ent_secondWord.Text))
            {
                DisplayAlert("Chyba", "Slovo nebo překlad obsahuje zakázané znaky (;:+=)!", "Ok");
            }
            else
            {
                selectedTranslate.FirstWord = ent_firstWord.Text;
                selectedTranslate.SecondWord = ent_secondWord.Text;
                if (pk_group.SelectedItem != null)
                {
                    int index = pk_group.SelectedIndex;
                    selectedTranslate.GroupName = pk_group.Items[index];
                }

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Translate>();
                    int rows = conn.Update(selectedTranslate);
                    if (rows > 0)
                    {
                        DisplayAlert("Úspěch", "Překlad byl úspěšně změněn!", "Ok");
                        ff.UpdateFirebaseUserGroups(false);
                        Navigation.PopAsync();
                    }
                    else
                    {
                        DisplayAlert("Chyba", "Překlad se nepovedlo změnit!", "Ok");
                    }
                }
            }               
        }

        private void btn_delete_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>();
                int rows = conn.Delete(selectedTranslate);
                if (rows > 0)
                {
                    DisplayAlert("Úspěch", "Překlad byl úspěšně smazán!", "Ok");
                    ff.UpdateFirebaseUserGroups(false);
                    Navigation.PopAsync();
                }
                else
                {
                    DisplayAlert("Chyba", "Překlad se nepovedlo smazat!", "Ok");
                }
            }
        }
    }
}
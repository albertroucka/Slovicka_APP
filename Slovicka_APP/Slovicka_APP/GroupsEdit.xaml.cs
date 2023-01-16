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
    public partial class GroupsEdit : ContentPage
    {
        MainClass mainClass = new MainClass(); FirebaseFirestore ff = new FirebaseFirestore();
        Group selectedGroup; string originalGroup;

        public GroupsEdit(Group selectedGroup)
        {
            InitializeComponent();
            this.selectedGroup = selectedGroup;
            this.originalGroup = selectedGroup.GroupName;
            ent_groupName.Text = selectedGroup.GroupName;
            pk_firstLang.Title = selectedGroup.FirstLang;
            pk_secondLang.Title = selectedGroup.SecondLang;
            List<string> languages = mainClass.GetAllLanguages();
            pk_firstLang.ItemsSource = languages;
            pk_secondLang.ItemsSource = languages;
        }

        private void btn_resetStats_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                selectedGroup.NumberOfExercises = 0; selectedGroup.SuccessRate = 100;
                conn.CreateTable<Group>();
                int rows = conn.Update(selectedGroup);
                if (rows > 0)
                {
                    DisplayAlert("Úspěch", "Statistiky byly úspěšně smazány!", "Ok");
                    ff.UpdateFirebaseUserGroups(false);
                    Navigation.PopAsync();
                }
                else
                {
                    DisplayAlert("Chyba", "Statistiky se nepovedlo smazat!", "Ok");
                }
            }
        }

        private void btn_update_Clicked(object sender, EventArgs e)
        {
            selectedGroup.GroupName = ent_groupName.Text;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>(); int i = 0; bool b = false;
                var groups = conn.Table<Group>().ToList();

                if (selectedGroup.GroupName != originalGroup)
                {
                    while (i < groups.Count)
                    {
                        var quest = groups[i];
                        if (quest.GroupName == selectedGroup.GroupName)
                        {
                            b = true;
                            break;
                        }
                        i++;
                    }
                }

                if (pk_firstLang.SelectedItem != null)
                {
                    int index = pk_firstLang.SelectedIndex;
                    string FirstLang = pk_firstLang.Items[index];
                    selectedGroup.FirstLang = FirstLang;
                }

                if (pk_secondLang.SelectedItem != null)
                {
                    int index = pk_secondLang.SelectedIndex;
                    string SecondLang = pk_secondLang.Items[index];
                    selectedGroup.SecondLang = SecondLang;
                }

                if (selectedGroup.FirstLang == selectedGroup.SecondLang)
                {
                    DisplayAlert("Chyba", "Musíte vybrat dva různé jazyky překladu!", "Ok");
                }
                else
                {
                    if (b == false)
                    {
                        int rows = conn.Update(selectedGroup);

                        conn.CreateTable<Translate>();
                        var posts = conn.Table<Translate>().ToList();
                        i = 0;
                        while (i < posts.Count)
                        {
                            var quest = posts[i];
                            if (quest.GroupName == originalGroup)
                            {
                                quest.GroupName = selectedGroup.GroupName;
                                rows = conn.Update(quest);
                            }
                            i++;
                        }

                        if (rows > 0)
                        {
                            DisplayAlert("Úspěch", "Název skupiny byl úspěšně změněn!", "Ok");
                            ff.UpdateFirebaseUserGroups(false);
                            Navigation.PopAsync();
                        }
                        else
                        {
                            DisplayAlert("Chyba", "Název skupiny se nepovedlo změnit!", "Ok");
                        }
                    }
                    else
                    {
                        DisplayAlert("Chyba", "Skupina se stejným názvem již existuje!", "Ok");
                    }
                }
            }
        }

        private void btn_delete_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>();
                var posts = conn.Table<Translate>().ToList();
                int i = 0;
                while (i < posts.Count)
                {
                    var quest = posts[i];
                    if (quest.GroupName == originalGroup)
                    {
                        conn.Delete(quest);
                    }
                    i++;
                }

                conn.CreateTable<Group>();
                int rows = conn.Delete(selectedGroup);
                if (rows > 0)
                {
                    DisplayAlert("Úspěch", "Skupina byla úspěšně smazána!", "Ok");
                    ff.UpdateFirebaseUserGroups(false);
                    Navigation.PopAsync();
                }
                else
                {
                    DisplayAlert("Chyba", "Skupinu se nepovedlo smazat!", "Ok");
                }
            }
        }
    }
}
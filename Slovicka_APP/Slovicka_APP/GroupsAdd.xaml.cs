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
    public partial class GroupsAdd : ContentPage
    {
        MainClass mainClass = new MainClass(); FirebaseFirestore ff = new FirebaseFirestore();

        public GroupsAdd()
        {
            InitializeComponent();
            List<string> languages = mainClass.GetAllLanguages();
            pk_firstLang.ItemsSource = languages;
            pk_secondLang.ItemsSource = languages;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                var groups = conn.Table<Group>().ToList();
                List<Group> sorted = groups.OrderBy(Group => Group.GroupName).ToList();
                lv_groups.ItemsSource = sorted;
            }
        }

        private void btn_saveGroup_Clicked(object sender, EventArgs e)
        {
            if (ent_groupName.Text != null)
            {
                if (pk_firstLang.SelectedItem == null || pk_secondLang.SelectedItem == null)
                {
                    DisplayAlert("Chyba", "Musíte vybrat jazyky překladu!", "Ok");
                }
                else if (pk_firstLang.SelectedItem == pk_secondLang.SelectedItem)
                {
                    DisplayAlert("Chyba", "Musíte vybrat dva různé jazyky překladu!", "Ok");
                }
                else
                {
                    int index = pk_firstLang.SelectedIndex;
                    string FirstLang = pk_firstLang.Items[index];
                    index = pk_secondLang.SelectedIndex;
                    string SecondLang = pk_firstLang.Items[index];

                    Group group = new Group() { GroupName = ent_groupName.Text, FirstLang = FirstLang, SecondLang = SecondLang, NumberOfExercises = 0, SuccessRate = 100 };

                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.CreateTable<Group>(); int i = 0; bool duplicate = false;
                        var groups = conn.Table<Group>().ToList();

                        while (i < groups.Count)
                        {
                            var quest = groups[i];
                            if (quest.GroupName == group.GroupName)
                            {
                                duplicate = true;
                                break;
                            }
                            i++;
                        }

                        if (duplicate == false)
                        {
                            int rows = conn.Insert(group);
                            if (rows > 0)
                            {
                                DisplayAlert("Úspěch", "Skupina byla úspěšně vytvořena!", "Ok");
                                ff.UpdateFirebaseUserGroups(true);
                                ent_groupName.Text = string.Empty;
                                pk_firstLang.SelectedItem = null;
                                pk_secondLang.SelectedItem = null;
                            }
                            else
                            {
                                DisplayAlert("Chyba", "Skupinu se nepovedlo vytvořit!", "Ok");
                            }
                        }
                        else
                        {
                            DisplayAlert("Chyba", "Skupina se stejným názvem již existuje!", "Ok");
                        }

                    }
                }
                OnAppearing();
            }
            else
            {
                DisplayAlert("Chyba", "Zadejte název skupiny!", "Ok");
            }
        }

        private void lv_groups_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedGroup = lv_groups.SelectedItem as Group;
            if (selectedGroup != null)
            {
                Navigation.PushAsync(new GroupsEdit(selectedGroup));
            }
        }

        private void GoToQRScannerPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new QRCodeScanner());
        }
    }
}
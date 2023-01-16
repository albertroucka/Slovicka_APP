using Plugin.CloudFirestore;
using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WordsGroup : ContentPage
    {
        Group selectedGroup; List<Translate> translatesList; FirebaseFirestore ff = new FirebaseFirestore(); MainClass mainClass = new MainClass();

        public WordsGroup(Group selectedGroup)
        {
            InitializeComponent();
            this.selectedGroup = selectedGroup;
            lb_groupName.Text = selectedGroup.GroupName;
            lb_numberOfExercises.Text = selectedGroup.NumberOfExercises.ToString();
            lb_numberOfSuccessRate.Text = Math.Round(selectedGroup.SuccessRate).ToString() + "%";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            translatesList = new List<Translate>();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>(); int i = 0;
                var posts = conn.Table<Translate>().ToList();

                while (i < posts.Count)
                {
                    var quest = posts[i];
                    if (quest.GroupName == selectedGroup.GroupName)
                    {
                        translatesList.Add(posts[i]);
                    }
                    i++;
                }

                cv_words.ItemsSource = translatesList;
                lb_numberOfTranslates.Text = translatesList.Count().ToString();
            }
        }

        private void cv_words_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPost = cv_words.SelectedItem as Translate;
            if (selectedPost != null)
            {
                Navigation.PushAsync(new WordsEdit(selectedGroup, selectedPost));
            }
        }

        private void GoToQRCreatePage_Clicked(object sender, EventArgs e)
        {
            if (translatesList.Count() > 0)
            {
                bool bannedChars = false;
                foreach (var item in translatesList)
                {
                    if (item.FirstWord.Contains(";") || item.SecondWord.Contains(";") || item.FirstWord.Contains(":") || item.SecondWord.Contains(":") || item.FirstWord.Contains("-") || item.SecondWord.Contains("-") || item.FirstWord.Contains("+") || item.SecondWord.Contains("+"))
                    {
                        bannedChars = true;
                    }
                }

                if (bannedChars)
                {
                    DisplayAlert("Chyba!", "Skupina obsahuje nepovolené znaky (;:-+)!", "Ok");
                    Navigation.PopAsync();
                }
                else
                {
                    string translates = string.Empty;
                    foreach (var item in translatesList)
                    {
                        string s = $"{item.FirstWord}-{item.SecondWord}";
                        translates = translates + " " + s + " +";
                    }
                    translates = translates.Remove(0,1);
                    translates = translates.Remove(translates.Length - 2);

                    GroupShare groupShare = new GroupShare()
                    {
                        AppName = "Slovicka_APP",
                        AppVersion = "0.7",
                        GroupCode = "",
                        GroupName = selectedGroup.GroupName,
                        FirstLang = selectedGroup.FirstLang,
                        SecondLang = selectedGroup.SecondLang,
                        Translates = translates
                    };

                    if (mainClass.CheckInternetConnection())
                    {
                        if (selectedGroup.GroupCode != null)
                        {
                            groupShare.GroupCode = selectedGroup.GroupCode;
                            UpdateGroupShare(groupShare);
                        }
                        else
                        {
                            groupShare.GroupCode = ff.GenereteGroupShareCode();
                            UploadGroupShare(groupShare, selectedGroup);
                            ff.UpdateFirebaseUserShareGroups();
                        }
                    }
                    else
                    {
                        DisplayAlert("Chyba", "Nejste připojeni k internetu!", "Ok");
                    }                 
                }
            }
            else
            {
                DisplayAlert("Chyba", "K přenosu skupiny musíte mít ve skupině alespoň jeden překlad!", "Ok");
            }
        }

        public async void UploadGroupShare(GroupShare groupShare, Group selectedGroup)
        {
            while (true)
            {
                var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("groups")
                                        .Document(groupShare.GroupCode)
                                        .GetAsync();

                if (document.Exists)
                {
                    groupShare.GroupCode = ff.GenereteGroupShareCode();
                }
                else
                {
                    break;
                }
            }

            await CrossCloudFirestore.Current
                    .Instance
                    .Collection("groups")
                    .Document(groupShare.GroupCode)
                    .SetAsync(groupShare);

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                selectedGroup.GroupCode = groupShare.GroupCode;
                int rows = conn.Update(selectedGroup);
                if (rows > 0) { }
            }

            Navigation.PushAsync(new QRCodeGenerator(groupShare));
        }

        public async void UpdateGroupShare(GroupShare groupShare)
        {
            await CrossCloudFirestore.Current
                         .Instance
                         .Collection("groups")
                         .Document(groupShare.GroupCode)
                         .UpdateAsync(groupShare);

            Navigation.PushAsync(new QRCodeGenerator(groupShare));
        }
    }
}
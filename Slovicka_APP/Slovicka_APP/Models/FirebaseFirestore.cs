using Plugin.CloudFirestore;
using Firebase.Auth;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Content.Res;
using Android.Views;
using Xamarin.Essentials;

namespace Slovicka_APP.Models
{
    public class FirebaseFirestore
    {
        public bool CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet) { return true; }
            else { return false; }
        }

        //Uživatelé a Firebase Auth
        string WebAPIkey = "AIzaSyDcNoIRjoK7vdlwK5_hJcCTjwon27x4nK8";
        public async void FirebaseLogIn(string username, string password, ActivityIndicator ai_loading)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(username, password);
                string getToken = auth.FirebaseToken;
                string authId = auth.User.LocalId;
                FirebaseGetUserData(authId, ai_loading);             
            }
            catch (FirebaseAuthException e)
            {
                string err = ShowFirebaseError(e.Message, false);
                await App.Current.MainPage.DisplayAlert("Chyba", err, "Ok");
            }
            catch (Exception e)
            {
                string err = ShowFirebaseError(e.Message, false);
                await App.Current.MainPage.DisplayAlert("Chyba", err, "Ok");
            }
        }

        private async void FirebaseGetUserData(string userAuthId, ActivityIndicator ai_loading)
        {
            var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("users")
                                        .Document(userAuthId)
                                        .GetAsync();

            if (document.Exists)
            {
                FirebaseUser firebaseUser = document.ToObject<FirebaseUser>();

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<LocalUser>();
                    var groups = conn.Table<LocalUser>().ToList();

                    LocalUser user = new LocalUser() { FirebaseId = firebaseUser.FirebaseId, UserName = firebaseUser.UserName, UserEmail = firebaseUser.UserEmail, NumberOfTrophies = firebaseUser.NumberOfTrophies, NumberOfExercises = firebaseUser.NumberOfExercises, NumberOfCreatedGroups = firebaseUser.NumberOfCreatedGroups, NumberOfSharedGroups = firebaseUser.NumberOfSharedGroups, RegistrationDate = firebaseUser.RegistrationDate, AllGroups = firebaseUser.AllGroups };
                    int rows = conn.Insert(user); 
                    if (rows > 0)
                    {
                        SaveAllUsersGroups(firebaseUser.AllGroups, false, false);
                        ai_loading.IsVisible = false;
                        App.Current.MainPage.Navigation.PopAsync();
                        App.Current.MainPage.Navigation.PushAsync(new UserDetail());
                        App.Current.MainPage.DisplayAlert("Úspěch", "Uživatel byl úspěšně přihlášen!", "Ok");
                    }
                }
            }
        }

        public async void FirebaseSignUp(string username, string email, string password, ActivityIndicator ai_loading)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                string getToken = auth.FirebaseToken; 
                await auth.UpdateProfileAsync(username, "");
                CreateFirebaseUser(username, email, auth.User.LocalId);
                var test = authProvider.SendEmailVerificationAsync(getToken);
                await App.Current.MainPage.DisplayAlert("Úspěch", "Registrace proběhla úspěšně! Na zadaný e-mail byl odeslán e-mail s odkazem k ověření účtu", "Ok");
            }
            catch (FirebaseAuthException e)
            {
                string err = ShowFirebaseError(e.Message, true);
                await App.Current.MainPage.DisplayAlert("Chyba", err, "Ok");
            }
            catch (Exception e)
            {
                string err = ShowFirebaseError(e.Message, true);
                await App.Current.MainPage.DisplayAlert("Chyba", err, "Ok");
            }
            ai_loading.IsVisible = false;
        }

        public async void FirebaseUserLogout(ActivityIndicator ai_loading)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count > 1)
                {
                    var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("users")
                                       .Document(users[1].FirebaseId)
                                       .GetAsync();

                    if (document.Exists)
                    {
                        FirebaseUser firebaseUser = document.ToObject<FirebaseUser>();
                        int rows = conn.Delete(users[1]);
                        if (rows > 0)
                        {
                            conn.CreateTable<Group>();
                            var groups = conn.Table<Group>().ToList();

                            foreach (var item in groups)
                            {
                                int rowsGroups = conn.Delete(item);
                            }

                            conn.CreateTable<Translate>();
                            var translates = conn.Table<Translate>().ToList();

                            foreach (var item in translates)
                            {
                                int rowsTranslates = conn.Delete(item);
                            }
                        }
                    }
                }
            }
            ai_loading.IsVisible = false;
            App.Current.MainPage.Navigation.PopAsync();
            App.Current.MainPage.Navigation.PopAsync();
            App.Current.MainPage.Navigation.PushAsync(new MainPage());
        }

        //Uživatel ve FirebaseFirestore
        private async void CreateFirebaseUser(string username, string email, string localId)
        {
            FirebaseUser firebaseUser = new FirebaseUser()
            {
                FirebaseId = localId,
                UserName = username,
                UserEmail = email,
                NumberOfTrophies = 0,
                NumberOfExercises = 0,
                NumberOfCreatedGroups = 0,
                NumberOfSharedGroups = 0,
                RegistrationDate = DateTime.Now,
                AllGroups = GetAllUserGroups()
            };

            await CrossCloudFirestore.Current
                         .Instance
                         .Collection("users")
                         .Document(localId)
                         .SetAsync(firebaseUser);

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var groups = conn.Table<LocalUser>().ToList();
                LocalUser localUser = new LocalUser() { FirebaseId = firebaseUser.FirebaseId, UserName = firebaseUser.UserName, UserEmail = firebaseUser.UserEmail, NumberOfTrophies = firebaseUser.NumberOfTrophies, NumberOfExercises = firebaseUser.NumberOfExercises, NumberOfCreatedGroups = firebaseUser.NumberOfCreatedGroups, NumberOfSharedGroups = firebaseUser.NumberOfSharedGroups, RegistrationDate = firebaseUser.RegistrationDate, AllGroups = firebaseUser.AllGroups };
                int rows = conn.Insert(localUser);
                if (rows > 0)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                    await App.Current.MainPage.Navigation.PushAsync(new UserDetail());
                }
            }
        }

        public async void UpdateFirebaseUser(FirebaseUser firebaseUser, ActivityIndicator ai_loading)
        {
            await CrossCloudFirestore.Current
             .Instance
             .Collection("users")
             .Document(firebaseUser.FirebaseId)
             .UpdateAsync(firebaseUser);

            if (ai_loading != null)
            {
                ai_loading.IsVisible = false;
            }
        }

        public async void DownloadFirebaseUserData(Button btn_account, Button btn_trophies)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count > 1)
                {
                    var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("users")
                                        .Document(users[1].FirebaseId)
                                        .GetAsync();

                    if (document.Exists)
                    {
                        FirebaseUser firebaseUser = document.ToObject<FirebaseUser>();
                        LocalUser user = new LocalUser() { FirebaseId = firebaseUser.FirebaseId, UserName = firebaseUser.UserName, UserEmail = firebaseUser.UserEmail, NumberOfTrophies = firebaseUser.NumberOfTrophies, NumberOfExercises = firebaseUser.NumberOfExercises, NumberOfCreatedGroups = firebaseUser.NumberOfCreatedGroups, NumberOfSharedGroups = firebaseUser.NumberOfSharedGroups, RegistrationDate = firebaseUser.RegistrationDate, AllGroups = firebaseUser.AllGroups, Id = users[1].Id };
                        int rows = conn.Update(user);
                        if (rows > 0)
                        {
                            SaveAllUsersGroups(firebaseUser.AllGroups, true, false);
                            btn_account.Text = user.UserName;
                            btn_trophies.Text = user.NumberOfTrophies.ToString();
                        }
                    }
                }
            }
        }

        public async void FirebaseUserResetPass(string email, ActivityIndicator ai_loading)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                await authProvider.SendPasswordResetEmailAsync(email);
                await App.Current.MainPage.Navigation.PopAsync();
                await App.Current.MainPage.DisplayAlert("Úspěch", "E-mail s odkazem pro obnovu hesla byl úspěšně odeslán!", "Ok");
            }
            catch (Exception e)
            {
                string err = ShowFirebaseError(e.Message, false);
                await App.Current.MainPage.DisplayAlert("Chyba", err, "Ok");
            }
            ai_loading.IsVisible = false;
        }

        private string ShowFirebaseError(string error, bool registration)
        { 
            if (error.Contains("MISSING_EMAIL")) { return "Zadejte e-mail!"; }
            else if (error.Contains("INVALID_EMAIL")) { return "Zadali jste neplatný e-mail!"; }
            else if (error.Contains("EMAIL_EXISTS")) { return "Uživatelský účet s tímto e-mailem již existuje!"; }
            else if (error.Contains("EMAIL_NOT_FOUND")) { return "Uživatelský účet s tímto e-mailem nebyl nalezen!"; }
            else if (error.Contains("MISSING_PASSWORD")) { return "Zadejte heslo!"; }
            else if (error.Contains("INVALID_PASSWORD")) { return "Zadali jste nesprávné heslo!"; }
            else if (error.Contains("WEAK_PASSWORD")) { return "Heslo musí mít alespoň 6 znaků!"; }
            else if (CheckInternetConnection() != true) { return "Chyba sítě, zkontrolujte své připojení k internetu!";  }
            else if (registration) { return "Při registraci došlo k chybě!"; }
            else { return "Při přihlašování došlo k chybě!"; }
        }

        public string GetAllUserGroups()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>();
                var groups = conn.Table<Group>().ToList();

                string allGroups = string.Empty;
                List<Translate> translatesList = new List<Translate>();

                foreach (var item in groups)
                {
                    translatesList.Clear();
                    conn.CreateTable<Translate>(); int i = 0;
                    var posts = conn.Table<Translate>().ToList();

                    while (i < posts.Count)
                    {
                        var quest = posts[i];
                        if (quest.GroupName == item.GroupName)
                        {
                            translatesList.Add(posts[i]);
                        }
                        i++;
                    }

                    string translates = string.Empty;
                    if (translatesList.Count > 0)
                    {
                        foreach (var item0 in translatesList)
                        {
                            string s = $"{item0.FirstWord}-{item0.SecondWord}";
                            translates = translates + " " + s + " +";
                        }
                        translates = translates.Remove(0, 1);
                        translates = translates.Remove(translates.Length - 2);
                    }

                    string str = $"{item.GroupName},{item.FirstLang},{item.SecondLang},{item.SuccessRate},{item.NumberOfExercises},{translates};";
                    allGroups = allGroups + str;
                }

                return allGroups;
            }
        }

        public void SaveAllUsersGroups(string allGroups, bool update, bool delete)
        {
            List<string> groups = new List<string>();

            while (allGroups.Length > 0)
            {
                int i0 = allGroups.IndexOf(";");
                string group = allGroups.Substring(0, i0);
                allGroups = allGroups.Replace($"{group};", "");
                groups.Add(group);
            }

            foreach (var item in groups)
            {
                string grp = item;
                int i = grp.IndexOf(",");
                string groupName = grp.Substring(0, i);
                grp = grp.Remove(0, i + 1);

                i = grp.IndexOf(",");
                string firstLang = grp.Substring(0, i);
                grp = grp.Remove(0, i + 1);

                i = grp.IndexOf(",");
                string secondLang = grp.Substring(0, i);
                grp = grp.Remove(0, i + 1);

                i = grp.IndexOf(",");
                string successRate = grp.Substring(0, i);
                grp = grp.Remove(0, i + 1);

                i = grp.IndexOf(",");
                string numberOfExercises = grp.Substring(0, i);
                grp = grp.Remove(0, i + 1);

                Group groupModel = new Group() { GroupName = groupName, FirstLang = firstLang, SecondLang = secondLang, SuccessRate = Convert.ToInt32(successRate), NumberOfExercises = Convert.ToInt32(numberOfExercises) };
                InsertNewGroup(groupModel, grp, update);
            }         
        }


        //Aktualizace/Update uživatelů
        public async void UpdateFirebaseUserGroups(bool newgroup)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>(); 
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count > 1)
                {
                    await CrossCloudFirestore.Current
                            .Instance
                            .Collection("users")
                            .Document(users[1].FirebaseId)
                            .UpdateAsync("AllGroups", GetAllUserGroups());

                    if (newgroup)
                    {
                        await CrossCloudFirestore.Current
                                .Instance
                                .Collection("users")
                                .Document(users[1].FirebaseId)
                                .UpdateAsync("NumberOfCreatedGroups", users[1].NumberOfCreatedGroups + 1);

                        LocalUser localUser = users[1];
                        localUser.NumberOfCreatedGroups = localUser.NumberOfCreatedGroups + 1;
                        int rows = conn.Update(localUser);
                        if (rows > 0)
                        {
                        }
                    }
                }
                else
                {
                    if (newgroup)
                    {
                        LocalUser localUser = users[0];
                        localUser.NumberOfCreatedGroups = localUser.NumberOfCreatedGroups + 1;
                        int rows = conn.Update(localUser);
                        if (rows > 0)
                        {
                        }
                    }
                }
            }
        }

        public async void UpdateFirebaseUserShareGroups()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalUser>();
                var users = conn.Table<LocalUser>().ToList();

                if (users.Count > 1)
                {
                    await CrossCloudFirestore.Current
                            .Instance
                            .Collection("users")
                            .Document(users[1].FirebaseId)
                            .UpdateAsync("NumberOfSharedGroups", users[1].NumberOfSharedGroups + 1);

                    LocalUser localUser = users[1];
                    localUser.NumberOfSharedGroups = localUser.NumberOfSharedGroups + 1;
                    int rows = conn.Update(localUser);
                    if (rows > 0)
                    {
                    }
                }
                else
                {
                    LocalUser localUser = users[0];
                    localUser.NumberOfSharedGroups = localUser.NumberOfSharedGroups + 1;
                    int rows = conn.Update(localUser);
                    if (rows > 0)
                    {
                    }
                }
            }
        }


        //Sdílení skupin
        //Ověřovní, nahrávání nebo aktualizování skupin v FF

        public void InsertNewGroup(Group newGroup, string translates, bool update)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Group>(); int i = 0;
                var groups = conn.Table<Group>().ToList();

                while (i < groups.Count)
                {
                    var quest = groups[i];
                    if (quest.GroupName == newGroup.GroupName)
                    {
                        while (quest.GroupName == newGroup.GroupName)
                        {
                            newGroup.GroupName = newGroup.GroupName + "x";
                        }
                    }
                    i++;
                }

                if (update)
                {
                    int rows = conn.Update(newGroup);
                    if (rows > 0) { }
                }
                else
                {
                    int rows = conn.Insert(newGroup);
                    if (rows > 0) { }
                }
            }

            InsertNewTranslates(newGroup.GroupName, translates, update);
        }

        public void InsertNewTranslates(string groupName, string allTranslates, bool update)
        {
            int index = allTranslates.IndexOf("+"); string str;
            List<string> translates = new List<string>();
            while (index > 0)
            {
                str = allTranslates.Substring(0, index - 1);
                allTranslates = allTranslates.Remove(0, index + 2);
                translates.Add(str);
                index = allTranslates.IndexOf("+");
            }

            if (allTranslates.Length > 1)
            {
                translates.Add(allTranslates);
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Translate>();

                foreach (var item in translates)
                {
                    int i = item.IndexOf("-");
                    string firstLang = item.Substring(0, i);
                    string secondLang = item.Remove(0, i + 1);

                    Translate newTranslate = new Translate()
                    {
                        GroupName = groupName,
                        FirstWord = firstLang,
                        SecondWord = secondLang
                    };

                    if (update)
                    {
                        int rows = conn.Update(newTranslate);
                        if (rows > 0) { }
                    }
                    else
                    {
                        int rows = conn.Insert(newTranslate);
                        if (rows > 0) { }
                    }
                }
            }
        }      

        public string GenereteGroupShareCode()
        {
            Random r = new Random(); string groupCode = string.Empty;
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int i = 0; i < 6; i++)
            {
                int type = r.Next(2);
                if (type == 0)
                {
                    groupCode += r.Next(0, 9);
                }
                else
                {
                    int i0 = alphabet.Length;
                    groupCode += alphabet[r.Next(0, 25)];
                }
            }

            return groupCode;
        }
    }
}

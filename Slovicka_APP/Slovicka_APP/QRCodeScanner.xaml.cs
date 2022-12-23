using Plugin.CloudFirestore;
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
    public partial class QRCodeScanner : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore();

        public QRCodeScanner()
        {
            InitializeComponent();
        }

        private void btn_codeConfirm_Clicked(object sender, EventArgs e)
        {
            string groupCode = ent_groupCode.Text.ToUpper();

             if (groupCode == null || groupCode.Length != 6)
            {
                DisplayAlert("Chyba", "Zadali jste špatný formát kódu!", "Ok");
            }
            else
            {
                CheckGroupShareCode(groupCode);
            }
        }

        private void btn_scanStart_Clicked(object sender, EventArgs e)
        {
            btn_scanStart.IsVisible = false;
            QRCodeScan.IsVisible = true;
            QRCodeScan.IsScanning = true;
        }

        private void QRCodeScan_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                QRCodeScan.IsScanning = false;
                string QRCode = result.ToString();
                string stringResult = Encoding.Unicode.GetString(Convert.FromBase64String(QRCode));

                int i = stringResult.IndexOf(";");
                string appName = stringResult.Substring(0, i);
                stringResult = stringResult.Remove(0, i + 1);

                i = stringResult.IndexOf(";");
                string appVersion = stringResult.Substring(1, i - 1);
                appVersion = appVersion.Replace("AppVersion: ", "");
                stringResult = stringResult.Remove(0, i + 1);

                i = stringResult.IndexOf(";");
                string groupCode = stringResult.Substring(1, i - 1);
                groupCode = groupCode.Replace("GroupCode: ", "");
                stringResult = stringResult.Remove(0, i + 1);

                i = stringResult.IndexOf(";");
                string groupName = stringResult.Substring(1, i - 1);
                groupName = groupName.Replace("GroupName: ", "");
                stringResult = stringResult.Remove(0, i + 1);

                CheckGroupShareCode(groupCode);
            });
        }

        public async void CheckGroupShareCode(string groupCode)
        {
            var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("groups")
                                        .Document(groupCode)
                                        .GetAsync();

            if (document.Exists)
            {
                var data = document.ToObject<GroupShare>();

                if (data.AppName == "Slovicka_APP")
                {
                    try
                    {
                        ff.InsertNewGroup(data);
                        DisplayAlert("Úspěch", "Skupina byla úspěšně načtena a přidána!", "Ok");
                        Navigation.PopAsync();
                    }
                    catch (Exception)
                    {
                        DisplayAlert("Chyba", "Při zpracování dat skupiny došlo k chybě!", "Ok");
                    }

                }
                else
                {
                    DisplayAlert("Chyba", "Při zpracování skupiny došlo k chybě!", "Ok");
                }
            }
            else
            {
                DisplayAlert("Chyba", "Skupina se zadaným kódem neexistuje!", "Ok");
            }
        }

        private void LoadFromFile_Clicked(object sender, EventArgs e)
        {

        }
    }
}
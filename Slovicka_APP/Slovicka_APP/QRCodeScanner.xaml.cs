using Plugin.CloudFirestore;
using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.QrCode.Internal;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodeScanner : ContentPage
    {
        FirebaseFirestore ff = new FirebaseFirestore(); MainClass mainClass = new MainClass();

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
                if (ff.CheckInternetConnection())
                {
                    CheckGroupShareCode(groupCode);
                }
                else
                {
                    DisplayAlert("Chyba", "Nejste připojeni k internetu!", "Ok");
                }
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
                ReadQRCode(QRCode);
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

                if (data.AppName == "Slovicka_APP" || data.AppName == "Slovicka_WEB")
                {
                    try
                    {
                        Group newGroup = new Group()
                        {
                            GroupName = data.GroupName,
                            FirstLang = data.FirstLang,
                            SecondLang = data.SecondLang,
                            NumberOfExercises = 0,
                            SuccessRate = 100
                        };

                        ff.InsertNewGroup(newGroup, data.Translates, false);
                        ff.UpdateFirebaseUserGroups(false);
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
                    DisplayAlert("Chyba", "Skupina má špatný formát!", "Ok");
                }
            }
            else
            {
                DisplayAlert("Chyba", "Skupina se zadaným kódem neexistuje!", "Ok");
            }
        }

        private async void LoadFromFile_Clicked(object sender, EventArgs e)
        {
            //DisplayAlert("Upozornění", "Tato funknce není zatím dostupná a bude přidána v následujících verzích", "Ok");

            FileResult file = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Vyberte soubor s QR kódem"
            });

            if (file != null)
            {
                Stream stream = await file.OpenReadAsync();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                List<GoogleVisionBarCodeScanner.BarcodeResult> obj = await GoogleVisionBarCodeScanner.Methods.ScanFromImage(bytes);

                if (obj.Count > 0 && obj[0] != null)
                {
                    string QRCode = obj[0].DisplayValue;
                    ReadQRCode(QRCode);
                    //await DisplayAlert("Úspěch", "QR byl úspěšně rozpoznán!", "Ok");
                }
                else
                {
                    //await Navigation.PushAsync(new GroupsOptions());
                    await DisplayAlert("Chyba", "QR kód nebyl nalezen!", "Ok");
                }
            }
        }

        private void ReadQRCode(string QRCode)
        {
            string stringResult = Encoding.UTF8.GetString(Convert.FromBase64String(QRCode));

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


            if (ff.CheckInternetConnection())
            {
                CheckGroupShareCode(groupCode);
            }
            else
            {
                DisplayAlert("Chyba", "Nejste připojeni k internetu!", "Ok");
            }
        }
    }
}
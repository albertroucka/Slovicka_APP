using Slovicka_APP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Slovicka_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodeGenerator : ContentPage
    {

        public QRCodeGenerator(GroupShare groupShare)
        {
            InitializeComponent();
            SetViews(groupShare);        
        }

        public void SetViews(GroupShare groupShare)
        {
            try
            {
                lb_groupCode.Text = groupShare.GroupCode;
                string QR = $"{groupShare.AppName}; AppVersion: {groupShare.AppVersion}; GroupCode: {groupShare.GroupCode}; GroupName: {groupShare.GroupName};";
                string QR_encrypted = Convert.ToBase64String(Encoding.Unicode.GetBytes(QR));
                QRCodeView.BarcodeValue = QR_encrypted;
            }
            catch (Exception)
            {
                DisplayAlert("Chyba", "Při sdílení skupiny došlo k chybě!", "Ok");
                Navigation.PopAsync();
            }
        }      
    }
}
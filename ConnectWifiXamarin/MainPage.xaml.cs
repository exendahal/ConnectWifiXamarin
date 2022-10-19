using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ConnectWifiXamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                var response = DependencyService.Get<IConnect>().ConnectWifi(ssid.Text, password.Text);
            }           
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                var response = DependencyService.Get<IConnect>().SuggestWifi(ssid.Text, password.Text);
            }
        }
    }
}

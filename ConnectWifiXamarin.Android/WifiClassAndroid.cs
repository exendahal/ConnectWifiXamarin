using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ConnectWifiXamarin.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(WifiClassAndroid))]
namespace ConnectWifiXamarin.Droid
{
    public class WifiClassAndroid : IConnect
    {
        private const int AddWifiSettingsRequestCode = 4242;
        public bool ConnectWifi(string ssid, string password)
        {
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                WifiManager wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
                if (!wifiManager.IsWifiEnabled)
                {
                    wifiManager.SetWifiEnabled(true);
                }
                String wifiSsid = wifiManager.ConnectionInfo.SSID.ToString();
                if (wifiSsid != string.Format("\"{0}\"", ssid))
                {
                    WifiConfiguration wifiConfig = new WifiConfiguration();
                    wifiConfig.Ssid = string.Format("\"{0}\"", ssid);
                    wifiConfig.PreSharedKey = string.Format("\"{0}\"", password);
                    int netId = wifiManager.AddNetwork(wifiConfig);
                    wifiManager.Disconnect();
                    wifiManager.EnableNetwork(netId, true);
                    wifiManager.Reconnect();
                    return true;
                }
            }
            else if(Build.VERSION.SdkInt == BuildVersionCodes.Q)
            {               
                MainActivity.Instance.SuggestNetwork(ssid, password);

            }
            else
            {
                AddWifi(ssid, password);
            }
            return false;
        }

        public bool SuggestWifi(string ssid, string password)
        {
            MainActivity.Instance.SuggestNetwork(ssid, password);
            return false;
        }

        private void AddWifi(string ssid, string psk)
        {
            var intent = new Intent("android.settings.WIFI_ADD_NETWORKS");
            var bundle = new Bundle();
            bundle.PutParcelableArrayList("android.provider.extra.WIFI_NETWORK_LIST",
                new List<IParcelable>
                {
                    new WifiNetworkSuggestion.Builder()
                        .SetSsid(ssid)
                        .SetWpa2Passphrase(psk)
                        .Build()
                });

            intent.PutExtras(bundle);

            MainActivity.Instance.StartActivityForResult(intent, AddWifiSettingsRequestCode);

        }

        
    }
}
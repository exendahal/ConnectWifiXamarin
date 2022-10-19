using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using static Android.Net.ConnectivityManager;
using Android.Net.Wifi;
using Android.Net;
using Android.Content;
using System.Linq;
using static Android.Provider.Settings;

namespace ConnectWifiXamarin.Droid
{
    [Activity(Label = "ConnectWifiXamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private bool _requested;
        private const int AddWifiSettingsRequestCode = 4242;
        internal static MainActivity Instance { get; set; }
        private NetworkCallback _callback;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);           
            LoadApplication(new App());
            _callback = new NetworkCallback
            {
                NetworkAvailable = network =>
                {
                    // we are connected!
                    //_statusText.Text = $"Request network available";
                },
                NetworkUnavailable = () =>
                {
                   // _statusText.Text = $"Request network unavailable";
                }
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestNetwork(string ssid, string password)
        {
            var specifier = new WifiNetworkSpecifier.Builder()
                .SetSsid(ssid)
                .SetWpa2Passphrase(password)
                .Build();

            var request = new NetworkRequest.Builder()
                .AddTransportType(TransportType.Wifi)
                .SetNetworkSpecifier(specifier)
                .Build();

            var connectivityManager = GetSystemService(ConnectivityService) as ConnectivityManager;

            if (_requested)
            {
                connectivityManager.UnregisterNetworkCallback(_callback);
            }

            connectivityManager.RequestNetwork(request, _callback);
            _requested = true;
        }
        public void SuggestNetwork(string ssid, string password)
        {
            var suggestion = new WifiNetworkSuggestion.Builder()
                .SetSsid(ssid)
                .SetWpa2Passphrase(password)
                .Build();

            var suggestions = new[] { suggestion };

            var wifiManager = GetSystemService(WifiService) as WifiManager;
            var status = wifiManager.AddNetworkSuggestions(suggestions);

            var statusText = status switch
            {
                NetworkStatus.SuggestionsSuccess => "Suggestion Success",
                NetworkStatus.SuggestionsErrorAddDuplicate => "Suggestion Duplicate Added",
                NetworkStatus.SuggestionsErrorAddExceedsMaxPerApp => "Suggestion Exceeds Max Per App"
            };
            OpenConnectivity();
           // _statusText.Text = statusText;
        }
        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            public Action<Network> NetworkAvailable { get; set; }
            public Action NetworkUnavailable { get; set; }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                NetworkAvailable?.Invoke(network);
            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
                NetworkUnavailable?.Invoke();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == AddWifiSettingsRequestCode)
            {
                if (data != null && data.HasExtra("android.provider.extra.WIFI_NETWORK_RESULT_LIST"))
                {
                    var extras =
                        data.GetIntegerArrayListExtra("android.provider.extra.WIFI_NETWORK_RESULT_LIST")
                            ?.Select(i => i.IntValue()).ToArray() ?? new int[0];

                    if (extras.Length > 0)
                    {
                        var ok = extras.Select(GetResultFromCode).All(r => r == Result.Ok);
                        //_result.Text = $"Result {ok}";
                        OpenConnectivity();
                        return;
                    }
                }
                OpenConnectivity();
               // _result.Text = $"Result {resultCode == Result.Ok}";
            }
        }

        private static Result GetResultFromCode(int code) =>
            code switch
            {
                0 => Result.Ok,
                2 => Result.Ok,
                _ => Result.Canceled
            };


        void OpenConnectivity()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                Intent panelIntent = new Intent(Panel.ActionWifi);
                MainActivity.Instance.StartActivityForResult(panelIntent, 545);
            }
        }
    }
}
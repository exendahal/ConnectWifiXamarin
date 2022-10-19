using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectWifiXamarin
{
    public interface IConnect
    {
        bool ConnectWifi(string ssid, string password);
        bool SuggestWifi(string ssid, string password);
    }
}

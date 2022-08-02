using Mixpanel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PureVPN.Updater
{
    public class MixpanelService
    {
        private static string token;
        private static string Token
        {
            get
            {
                if (String.IsNullOrEmpty(token))
                    token = "0f2e8ba3e39bda1226054ba0b84c7146";// "83fce825eb6aec8b34318bbca22f1033";
                return token;
            }
            set
            {
                token = value;
            }
        }

        private static MixpanelClient MixPanelClient { get { return new MixpanelClient(Token); } }

        public async Task<bool> Fire(string eventName, string userName, Dictionary<string, object> properties)
        {
            return await MixPanelClient.TrackAsync(eventName, userName, properties);
        }
    }


}

using Mixpanel;
using PureVPN.Service;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using Sentry;
using Sentry.Extensibility;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;




namespace PureVPN.Service
{
    public class MixpanelService : IMixpanelService
    {

        private ISentryService sentryService;

        public MixpanelService(ISentryService _sentryService)
        {
            this.sentryService = _sentryService;
        }


        private static string token;
        public static string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }


        private static HashSet<string> ConnectionRelatedEvents = new HashSet<string>
        {

            "app_disconnected",
            "app_connected",
            "app_connect",
            "app_cancelled",
            "app_utb",
            "app_utc",
            "app_ttc"
        };

        private static MixpanelClient MixPanelClient { get { return new MixpanelClient(Token); } }

        public void FireEvent(string eventName, string hostingId, Dictionary<string, object> properties = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (!Service.Helper.Common.MuteTracker)
                    {
                        if (!Common.IsExecuting)
                            await SendLeftOverEvents();

                        if (properties == null)
                            properties = new MixpanelProperties().MixPanelPropertiesDictionary;

                        hostingId = !string.IsNullOrEmpty(hostingId) ? hostingId : "-1";

                        var result = await Fire(eventName, hostingId, properties);

                        if (!result)
                            SendLater(eventName, hostingId, properties);

                        // return result;
                    }
                    // else
                    //   return true;
                }
                catch (Exception ex)
                {
                    // sentryService.LoggingException(ex);
                }
            });

            // return false;
        }


        private void SendLater(string eventName, string username, Dictionary<string, object> properties)
        {
            try
            {
                if (Common.LeftOverEvents == null)
                    Common.LeftOverEvents = new Dictionary<string, Dictionary<string, object>>();

                if (Common.LeftOverEvents.ContainsKey(eventName))
                {
                    Common.LeftOverEvents[eventName] = properties;
                }
            }
            catch (Exception ex)
            {
                //sentryService.LoggingException(ex);
            }
        }


        private async Task SendLeftOverEvents()
        {
            Common.IsExecuting = true;

            try
            {

                if (Common.LeftOverEvents == null || Common.LeftOverEvents.Count <= 0)
                {
                    Common.IsExecuting = false;
                    return;
                }


                var tempEvents = new Dictionary<string, Dictionary<string, object>>();
                foreach (var evt in Common.LeftOverEvents)
                {
                    var username = string.IsNullOrEmpty(PureVPN.Entity.Models.AtomModel.HostingID) ? "-1" : PureVPN.Entity.Models.AtomModel.HostingID;

                    var fired = await Fire(evt.Key, username, evt.Value);

                    if (!fired)
                        tempEvents[evt.Key] = evt.Value;
                }

                Common.LeftOverEvents = tempEvents;

            }
            catch (Exception ex)
            {
                // sentryService.LoggingException(ex);
            }
            Common.IsExecuting = false;
        }


        private async Task<bool> Fire(string eventName, string hostingId, Dictionary<string, object> properties)
        {
            try
            {
                if (!Service.Helper.Common.MuteTracker)
                {
                    var manipulate_time = ConnectionRelatedEvents.FirstOrDefault(x => x.Contains(eventName)) != null;
                    return await MixPanelClient.TrackAsync(eventName, hostingId, properties, manipulate_time);
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                // sentryService.LoggingException(ex);
                return true;
            }
        }
    }


}

using PureVPN.Entity.Models;
using PureVPN.Realtime.Repository.Infrastructure.Models;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class RealtimeConnectionStringProvider : IConnectionStringProvider
    {
        /// <summary>
        /// Firebase Project Id
        /// </summary>
        public string ProjectId => Common.RealtimeDatabase.ProjectId;

        /// <summary>
        /// Firebase API Key
        /// </summary>
        public string APIKey => Encoding.ASCII.GetString(Convert.FromBase64String(Common.RealtimeDatabase.APIKey));

        /// <summary>
        /// Collection Name
        /// </summary>
        public string Collection => Common.RealtimeDatabase.TroubleshootColllectionName;

        public IDialerToServerService DialerToServerService { get; }
        public ISentryService Logger { get; }

        public RealtimeConnectionStringProvider(IDialerToServerService dialerToServerService, ISentryService logger)
        {
            DialerToServerService = dialerToServerService;
            Logger = logger;
        }

        /// <summary>
        /// Provides Custom token for User
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCustomTokenAsync()
        {
            try
            {
                var tokenReply = await DialerToServerService.GetFirestoreTokenAsync(AtomModel.UUID, AtomModel.Username);
                return tokenReply?.Body?.Token;
            }
            catch (Exception ex)
            {
                Logger.LoggingException(ex);
                return null;
            }
        }
    }
}

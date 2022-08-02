using Newtonsoft.Json;
using PureVPN.Entity.Models.DTO;
using PureVPN.Realtime.Repository.Infrastructure;
using PureVPN.Service.Contracts;
using PureVPN.Service.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class TroubleshootErrorService : ITroubleshootErrorService
    {
        #region ctor & properties
        public TroubleshootErrorService(IRealtimeRepository realtimeRepository, ISentryService sentryService)
        {
            RealtimeRepository = realtimeRepository;
            SentryService = sentryService;
        }

        public IRealtimeRepository RealtimeRepository { get; }
        public ISentryService SentryService { get; }

        #endregion

        #region ITroubleshootErrorService implementations
        /// <summary>
        /// Start getting realtime upadtes to toubleshoot errors
        /// </summary>
        public void GetRealtimeChanges()
        {
            try
            {
                RealtimeRepository.OnDataChanges<Entity.Models.DTO.Atom>(OnDataChange, Common.RealtimeDatabase.TroubleshootColllectionName);
                Common.FirestoreDb = RealtimeRepository.GetDatabaseConext();
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
            }
            catch { }
        }
        #endregion


        #region Private methods

        /// <summary>
        /// On Data Change
        /// </summary>
        /// <param name="entity"></param>
        private void OnDataChange(Entity.Models.DTO.Atom entity)
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorMessages");
            if (!directory.TryCreateDirectory())
            {
                return;
            }

            var filename = Path.Combine(directory, $"{entity.Code.ToString()}.json");
            if (entity.IsDeleted)
            {
                filename.DeleteFile();
            }
            else
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(entity));
            }
        }

        #endregion
    }
}

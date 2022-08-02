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
    public class DislikeReasonsService : IDislikeReasonsService
    {
        public DislikeReasonsService(IRealtimeRepository realtimeRepository, ISentryService sentryService)
        {
            RealtimeRepository = realtimeRepository;
            SentryService = sentryService;
            this.DislikeReasonsFilename = $"{Common.RealtimeDatabase.DislikeReasonsCollection}_{Common.RealtimeDatabase.DislikeReasonsQuery}";
            this.Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CachedFiles");
        }

        public IRealtimeRepository RealtimeRepository { get; }
        public ISentryService SentryService { get; }
        public string DislikeReasonsFilename { get; private set; }
        public string Directory { get; private set; }

        /// <summary>
        /// Gets list of Dislike Reasons
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DislikeReasonModel> GetDislikeReasons()
        {
            var filename = GetCompleteFilename();
            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                var dislikeReasons = JsonConvert.DeserializeObject<DislikeReasonsModel>(json);
                return dislikeReasons?.Reasons;
            }
            return null;
        }

        /// <summary>
        /// Start getting realtime updates to dislike reasons
        /// </summary>
        public void GetRealtimeChanges()
        {
            try
            {
                RealtimeRepository.OnDataChanges<DislikeReasonsModel>(OnDataChange,
                    Common.RealtimeDatabase.DislikeReasonsCollection, Common.RealtimeDatabase.DislikeReasonsQuery);
                Common.FirestoreDb = RealtimeRepository.GetDatabaseConext();
            }
            catch (Exception ex)
            {
                SentryService.LoggingException(ex);
            }
            catch { }
        }


        /// <summary>
        /// On Data Change
        /// </summary>
        /// <param name="entity"></param>
        private void OnDataChange(Entity.Models.DTO.DislikeReasonsModel entity)
        {
            if (!Directory.TryCreateDirectory())
            {
                return;
            }

            var filename = GetCompleteFilename();
            if (entity.IsDeleted == false)
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(entity));
            }
        }

        private string GetCompleteFilename()
        {
            return Path.Combine(Directory, DislikeReasonsFilename);
        }
    }
}
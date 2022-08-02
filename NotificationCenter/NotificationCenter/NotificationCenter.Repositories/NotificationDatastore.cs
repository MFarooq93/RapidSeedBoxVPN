using Google.Cloud.Firestore;
using NotificationCenter.Infrastructure.Contracts;
using NotificationCenter.Infrastructure.Extensions;
using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Repositories
{
    public class NotificationDatastore : INotificationDatastore
    {
        public NotificationDatastore(IFirebaseConfiguration firebaseConfiguration)
        {
            this.QueryPath = firebaseConfiguration.QueryPath;
            this.FirestoreDb = firebaseConfiguration.FirestoreDb as FirestoreDb;
        }

        public string QueryPath { get; }
        public FirestoreDb FirestoreDb { get; }
        public FirestoreChangeListener QueryListener { get; private set; }
        public INotificationCenterErrorLogger ErrorLogger { get; set; }

        public void GetDataChanges(Action<List<DataChangeDTO<NotificationDTO>>> callback)
        {
            try
            {
                this.QueryListener = FirestoreDb.Collection(QueryPath).Listen((snapshot) =>
                    {
                        try
                        {
                            var changes = new List<DataChangeDTO<NotificationDTO>>();

                            var snapshotChanges = snapshot.Changes;

                            if (snapshotChanges == null)
                            {
                                return;
                            }

                            foreach (var item in snapshotChanges)
                            {
                                NotificationDTO entity = new NotificationDTO();
                                try
                                {
                                    entity.Id = item.Document.Id;
                                    entity.Clone(item.Document.ToDictionary());
                                }
                                catch (Exception ex)
                                {
                                    ErrorLogger?.LogError(ex);
                                }
                                var change = new DataChangeDTO<NotificationDTO>()
                                {
                                    NotificationChangeType = GetChangeType(item.ChangeType),
                                    Data = entity
                                };
                                changes.Add(change);
                            }
                            callback(changes);
                        }
                        catch (Exception ex)
                        {
                            this.ErrorLogger.LogError(ex);
                        }
                        catch { }
                    });
            }
            catch (Exception ex)
            {
                this.ErrorLogger.LogError(ex);
            }
            catch { }
        }

        private static NotificationChangeType GetChangeType(DocumentChange.Type docChangeType)
        {
            NotificationChangeType response = default(NotificationChangeType);
            switch (docChangeType)
            {
                case DocumentChange.Type.Added:
                    response = NotificationChangeType.Added;
                    break;
                case DocumentChange.Type.Removed:
                    response = NotificationChangeType.Deleted;
                    break;
                case DocumentChange.Type.Modified:
                    response = NotificationChangeType.Updated;
                    break;
            }
            return response;
        }

        /// <summary>
        /// Get all <see cref="NotificationDTO"/>s
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<NotificationDTO>> GetAll()
        {
            var response = new List<NotificationDTO>();
            try
            {
                var snapshot = await FirestoreDb.Collection(QueryPath).GetSnapshotAsync();
                foreach (var item in snapshot.Documents)
                {
                    NotificationDTO notification = new NotificationDTO();
                    try
                    {
                        notification.Id = item.Id;
                        notification.Clone(item.ToDictionary());
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger?.LogError(ex);
                    }
                    response.Add(notification);
                }
            }
            catch (Exception ex)
            {
                this.ErrorLogger.LogError(ex);
            }
            catch { }
            return response;
        }

        public void Dispose()
        {
            try
            {
                this.QueryListener?.StopAsync();
                this.QueryListener = null;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
            catch { }
        }
    }
}
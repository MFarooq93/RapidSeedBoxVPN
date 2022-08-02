using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Realms;
using NotificationCenter.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace NotificationCenter.Repositories
{
    public class Repository<T> : IRepository<T> where T : RealmObject
    {
        #region ctor & properties
        public Repository(IDatabaseProvider databaseProvider)
        {
            DatabaseProvider = databaseProvider;
        }

        /// <summary>
        /// Context to the database
        /// </summary>
        protected Realm Database
        {
            get
            {
                return Realm.GetInstance(new RealmConfiguration(DatabaseProvider.DatabasePath) { ShouldDeleteIfMigrationNeeded = true });
            }
        }

        public IDatabaseProvider DatabaseProvider { get; }
        public Action<List<DataChangeDTO<T>>> DataChangeCallback { get; protected set; }
        public IDisposable Subscriber { get; protected set; }
        #endregion

        #region IRepository implementations

        /// <summary>
        /// Gets All <see cref="T"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return Database.All<T>().AsEnumerable();
        }

        /// <summary>
        /// Gets <see cref="T"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(string id)
        {
            return Database.Find<T>(id);
        }

        /// <summary>
        /// Gets <see cref="IEnumerable{T}"/> filtered with <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return Database.All<T>().Where(predicate);
        }

        /// <summary>
        /// Adds or update <see cref="T"/> <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Upsert(T entity)
        {
            var realm = Database;
            using (var transaction = realm.BeginWrite())
            {
                var respnose = realm.Add<T>(entity, update: true);
                transaction.Commit();
                return respnose;
            }
        }

        /// <summary>
        /// Removes an <see cref="T"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id)
        {
            var realm = Database;
            using (var transaction = realm.BeginWrite())
            {
                var objc = Get(id);
                if (objc != null)
                {
                    realm.Remove(objc);
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Deletes all records of Type <see cref="T"/> & inserts <paramref name="entities"/>
        /// </summary>
        /// <param name="entities"></param>
        public void TruncateAndDump(IEnumerable<T> entities)
        {
            var realm = Database;
            using (var transaction = realm.BeginWrite())
            {
                try
                {
                    realm.RemoveAll<T>();
                    foreach (var item in entities)
                    {
                        realm.Add(item);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public void OnChange(Action<List<DataChangeDTO<T>>> callback)
        {
            this.DataChangeCallback = callback;
            this.Subscriber = this.Database.All<T>().SubscribeForNotifications(OnLocalDataChanged);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Database.All<T>().Where(predicate).Count();
        }
        #endregion


        protected void OnLocalDataChanged(IRealmCollection<T> sender, ChangeSet changes, Exception error)
        {
            if (error != null)
            {
                // TODO: Log Error
                return;
            }

            if (changes == null)
            {
                return;
            }

            List<DataChangeDTO<T>> allChanges = new List<DataChangeDTO<T>>();

            allChanges.AddRange(GetDataChangeDTObyIndices(sender, changes.InsertedIndices, NotificationChangeType.Added));
            allChanges.AddRange(GetDataChangeDTObyIndices(sender, changes.ModifiedIndices, NotificationChangeType.Updated));
            allChanges.AddRange(GetDataChangeDTObyIndices(sender, changes.DeletedIndices, NotificationChangeType.Deleted));

            this.DataChangeCallback?.Invoke(allChanges);
        }

        private List<DataChangeDTO<T>> GetDataChangeDTObyIndices(IRealmCollection<T> sender, int[] indices, NotificationChangeType notificationChangeType)
        {
            List<DataChangeDTO<T>> changes = new List<DataChangeDTO<T>>();
            if (indices != null)
            {
                foreach (var index in indices)
                {
                    var item = sender.ElementAtOrDefault(index);
                    if (item != null)
                    {
                        var dto = new DataChangeDTO<T>() { Data = item, NotificationChangeType = notificationChangeType };
                        changes.Add(dto);
                    }
                }
            }
            return changes;
        }

        public void Dispose()
        {
            this.Subscriber?.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using PureVPN.Realtime.Repository.Infrastructure.Models;

namespace PureVPN.Realtime.Repository.Infrastructure
{
    /// <summary>
    /// Provides methods to access real time data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRealtimeRepository
    {
        /// <summary>
        /// Initializes with the credentials of logged in user
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        /// Already initialized <see cref="FirestoreDb"/> instance
        /// </summary>
        object GetDatabaseConext();

        /// <summary>
        /// Set <paramref name="action"/> to perform when data changes
        /// </summary>
        /// <param name="action">Action to perform when data is changed</param>
        void OnDataChanges<T>(Action<T> action, string collection) where T : IEntity;

        /// <summary>
        /// Adds listener to detect changes to a record and Sets action to trigger on change
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        void OnDataChanges<T>(Action<T> action, string collection, string id) where T : IEntity;

        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll<T>(string collection) where T : IEntity;

        /// <summary>
        /// Get <typeparamref name="T"/> by <see cref="IEntity.Id"/>
        /// </summary>
        /// <param name="id">Unique Id for records</param>
        /// <returns></returns>
        Task<T> Get<T>(string id, string collection) where T : IEntity;

        /// <summary>
        /// Stops all listeners
        /// </summary>
        Task StopListeners();
    }
}
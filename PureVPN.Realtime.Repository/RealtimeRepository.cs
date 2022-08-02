using PureVPN.Realtime.Repository.Infrastructure;
using PureVPN.Realtime.Repository.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Apis.Requests;
using Google.Apis.Http;
using Google.Apis.Util;
using System.Net.Http;
using Grpc.Core;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;

namespace PureVPN.Realtime.Repository
{
    /// <summary>
    /// Implements <see cref="IRealtimeRepository{T}"/> to access Realtime data
    /// </summary>
    /// <typeparam name="T">Model for data</typeparam>
    public class RealtimeRepository : IRealtimeRepository
    {
        /// <summary>
        /// Base URL for Firebase Aith APIs
        /// </summary>
        private const string _BASE_URL = "https://identitytoolkit.googleapis.com/v1/";

        #region crot and properties
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        public RealtimeRepository(IConnectionStringProvider connectionStringProvider)
        {
            ConnectionStringProvider = connectionStringProvider;
            this.Initialize().Wait();
        }

        /// <summary>
        /// Connection string provider
        /// </summary>
        public IConnectionStringProvider ConnectionStringProvider { get; }

        /// <summary>
        /// Context to the Database
        /// </summary>
        public FirestoreDb Database { get; private set; }

        public Dictionary<string, FirestoreChangeListener> QueryListeners { get; private set; } =
            new Dictionary<string, FirestoreChangeListener>();

        #endregion


        #region IRealtimeRepository implementations
        /// <summary>
        /// Already initialized <see cref="FirestoreDb"/> instance
        /// </summary>
        public object GetDatabaseConext()
        {
            return Database;
        }

        /// <summary>
        /// Get <typeparamref name="T"/> by <see cref="IEntity.Id"/>
        /// </summary>
        /// <param name="id">Unique Id for records</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string id, string collection) where T : IEntity
        {
            var db = await GetFirestoreDb();
            DocumentReference docRef = db.Collection(collection).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<T>() : default(T);
        }

        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll<T>(string collection) where T : IEntity
        {
            var response = new List<T>();
            var db = await GetFirestoreDb();
            var snapshot = await db.Collection(collection).GetSnapshotAsync();
            foreach (var item in snapshot.Documents)
            {
                response.Add(item.ConvertTo<T>());
            }
            return response;
        }

        /// <summary>
        /// Adds listener to detect change to a collection and Sets <paramref name="action"/> to perform when data changes
        /// </summary>
        /// <param name="action">Action to perform when data is changed</param>
        public void OnDataChanges<T>(Action<T> action, string collection) where T : IEntity
        {
            StopListener(collection);
            var db = GetFirestoreDb().Result;

            this.QueryListeners[collection] = db.Collection(collection).Listen((snapshot) =>
            {
                var changes = snapshot.Changes;

                if (changes == null)
                {
                    return;
                }

                foreach (var item in changes)
                {
                    T entity = default(T);
                    try
                    {
                        entity = Activator.CreateInstance<T>();
                        entity.Id = item.Document.Id;
                        entity.IsDeleted = item.ChangeType == Google.Cloud.Firestore.DocumentChange.Type.Removed;
                        entity.Clone(item.Document.ToDictionary());
                    }
                    catch { }
                    action(entity);
                }
            });
        }

        /// <summary>
        /// Adds listener to detect changes to a record and Sets action to trigger on change
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        public void OnDataChanges<T>(Action<T> action, string collection, string id) where T : IEntity
        {
            StopListener(collection);
            var db = GetFirestoreDb().Result;

            this.QueryListeners[collection] = db.Collection(collection).Document(id).Listen((snapshot) =>
            {
                if (snapshot == null)
                {
                    return;
                }

                T entity = default(T);
                try
                {
                    entity = Activator.CreateInstance<T>();
                    entity.Id = snapshot?.Id;
                    entity.IsDeleted = !snapshot.Exists;
                    entity.Clone(snapshot?.ToDictionary());
                }
                catch { }
                action(entity);
            });
        }


        /// <summary>
        /// Initializes with the credentials of logged in user
        /// </summary>
        /// <param name="connectionStringProvider"></param>
        public async Task Initialize()
        {
            await this.StopListeners();
            this.Database = await GetFirestoreDb(this.ConnectionStringProvider);
        }

        /// <summary>
        /// Stops all listeners
        /// </summary>
        public async Task StopListeners()
        {
            if (this.QueryListeners != null)
                foreach (var item in this.QueryListeners)
                    try { await item.Value.StopAsync(); } catch { }
            this.QueryListeners?.Clear();
            this.Database = null;
        }

        #endregion


        #region private methods

        /// <summary>
        /// Stop listener
        /// </summary>
        private void StopListener(string queryPath)
        {
            if (this.QueryListeners.ContainsKey(queryPath))
            {
                try
                {
                    this.QueryListeners[queryPath]?.StopAsync()?.Wait();
                }
                catch { }
            }
        }

        /// <summary>
        /// Get <see cref="FirestoreDb"/> for <paramref name="connectionStringProvider"/>
        /// </summary>
        /// <param name="connectionStringProvider"></param>
        /// <returns></returns>
        private async Task<FirestoreDb> GetFirestoreDb(IConnectionStringProvider connectionStringProvider)
        {
            var builder = new FirestoreDbBuilder();
            builder.ProjectId = ConnectionStringProvider.ProjectId;

            var customToken = await ConnectionStringProvider.GetCustomTokenAsync();
            var oauthToken = await SignInWithCustomToken(customToken, ConnectionStringProvider.APIKey);
            ITokenAccess credential = GoogleCredential.FromAccessToken(oauthToken);
            builder.TokenAccessMethod = credential.GetAccessTokenForRequestAsync;

            return builder.Build();
        }

        /// <summary>
        /// Sign in with Custom token
        /// </summary>
        /// <param name="customToken"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        private async Task<string> SignInWithCustomToken(string customToken, string apiKey)
        {
            var requestBuilder = new RequestBuilder
            {
                Method = HttpConsts.Post,
                BaseUri = new Uri(_BASE_URL),
                Path = "accounts:signInWithCustomToken"
            };
            requestBuilder.AddParameter(RequestParameterType.Query, "key", apiKey);
            var request = requestBuilder.CreateRequest();
            var jsonSerializer = Google.Apis.Json.NewtonsoftJsonSerializer.Instance;
            var payload = jsonSerializer.Serialize(new
            {
                token = customToken,
                returnSecureToken = true,
            });
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var parsed = jsonSerializer.Deserialize<dynamic>(json);
                return parsed.idToken;
            }
        }

        /// <summary>
        /// Get <see cref="FirestoreDb"/>
        /// </summary>
        /// <returns></returns>
        private async Task<FirestoreDb> GetFirestoreDb()
        {
            if (Database == null)
            {
                Database = await GetFirestoreDb(this.ConnectionStringProvider);
            }
            return Database;
        }
        #endregion
    }
}

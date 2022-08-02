using NotificationCenter.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Repositories
{
    public class DatabaseProvider : IDatabaseProvider
    {
        /// <summary>
        /// Directory name for data
        /// </summary>
        private const string _DIRECTORY = "AppData";
        /// <summary>
        /// Database filename for notifications database
        /// </summary>
        private const string _FILENAME = "notifications.realm";

        public DatabaseProvider()
        {
            // TODO: Handle failure to create database file
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _DIRECTORY);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var path = Path.Combine(directory, _FILENAME);
            DatabasePath = path;
        }

        /// <summary>
        /// Absoulte path to  the database
        /// </summary>
        public string DatabasePath { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Repositories
{
    public interface IDatabaseProvider
    {
        /// <summary>
        /// Absoulte path to  the database
        /// </summary>
        string DatabasePath { get; }
    }
}

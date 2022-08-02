using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Realtime.Repository.Infrastructure.Models
{
    /// <summary>
    /// Describes Credential Types supported by PureVPN.Realtime.Repository
    /// </summary>
    public enum CredentialType
    {
        /// <summary>
        /// Service account credentials in JSON format, provides application level access
        /// </summary>
        Json,
        /// <summary>
        /// Authentication token generated with User level access
        /// </summary>
        [Obsolete("User level authentication is not supported currently, please use CredentialType.Json or CredentialType.FilePath")]
        Token,
        /// <summary>
        /// Absolute path to Service account credentials in JSON format, provides application level access
        /// </summary>
        FilePath
    }
}

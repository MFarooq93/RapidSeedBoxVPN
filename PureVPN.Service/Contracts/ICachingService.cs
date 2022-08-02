using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    /// <summary>
    /// Provide methods to maintain cache for views
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Initializes the caching mehanism
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get content by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id for content</param>
        /// <returns>Content from cached file for <paramref name="id"/></returns>
        string GetContent(string id);
    }
}

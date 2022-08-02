using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    /// <summary>
    /// Represents App Language
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
        /// Display name for language
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Culture code for <see cref="LanguageModel"/>
        /// </summary>
        public string CultureCode { get; set; }
        public string Flag
        {
            get { return !string.IsNullOrEmpty(CultureCode) ? $"pack://application:,,,/assets/RapidSeedBoxImages/LanguageImg/{CultureCode.ToLower()}.png" : ""; }
        }
        public bool IsFavorite { get; set; }
    }
}

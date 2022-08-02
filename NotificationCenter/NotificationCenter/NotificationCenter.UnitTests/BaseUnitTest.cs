using NotificationCenter.Builders;
using NotificationCenter.Infrastructure;
using NotificationCenter.Infrastructure.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.UnitTests
{
    /// <summary>
    /// Provides base functionalities for unit tests
    /// </summary>
    public abstract class BaseUnitTest
    {
        /// <summary>
        /// Instantiates <see cref="INCManager"/>
        /// </summary>
        /// <returns></returns>
        public INCManager GetNCManager()
        {
            INCManagerBuilder builder = new NCManagerBuilder();
            return builder.Build();
        }
    }
}
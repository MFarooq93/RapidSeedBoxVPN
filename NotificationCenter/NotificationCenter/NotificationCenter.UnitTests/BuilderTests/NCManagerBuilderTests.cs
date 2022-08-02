using NotificationCenter.Builders;
using NotificationCenter.Infrastructure;
using NotificationCenter.Infrastructure.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NotificationCenter.UnitTests.BuilderTests
{
    public class NCManagerBuilderTests : BaseUnitTest
    {
        /// <summary>
        /// Tests <see cref="NCManagerBuilder"/> to instantiate <see cref="INCManager"/> without any parameters
        /// </summary>
        [Fact]
        public void NCManagerBuilder_WithNoParameters_ReturnNCManager()
        {
            // Arrange
            INCManagerBuilder builder = new NCManagerBuilder();

            // Act
            INCManager manager = builder.Build();

            // Assert
            Assert.NotNull(manager);
        }
    }
}

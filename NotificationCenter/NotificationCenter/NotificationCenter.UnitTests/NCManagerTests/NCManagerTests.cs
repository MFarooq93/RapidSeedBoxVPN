using NotificationCenter.Infrastructure.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NotificationCenter.UnitTests.NCManagerTests
{
    /// <summary>
    /// Tests APIs exposed from <see cref="NCManager"/>
    /// </summary>
    public class NCManagerTests : BaseUnitTest
    {
        /// <summary>
        /// Test AreYouThere API of <see cref="NCManager"/> which is expected to return <see cref="string"/> I am here
        /// </summary>
        [Fact]
        public void NCManager_AreYouThereAPI_ReturnIAmHere()
        {
            // Arrange
            var manager = GetNCManager();
            var expectedOutput = "I am here";

            // Act
            var actualOutput = manager.AreYouThere();

            // Assert
            Assert.Equal(expectedOutput, actualOutput);
        }
    }
}

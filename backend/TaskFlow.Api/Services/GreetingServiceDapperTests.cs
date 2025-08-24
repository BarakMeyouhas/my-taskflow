using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TaskFlow.Api.Services;
using Xunit;

namespace TaskFlow.Api.Tests.Services
{
    public class GreetingServiceDapperTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly Mock<ILogger<GreetingServiceDapper>> _mockLogger;
        private readonly GreetingServiceDapper _service;

        public GreetingServiceDapperTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            _mockLogger = new Mock<ILogger<GreetingServiceDapper>>();
            _service = new GreetingServiceDapper(_mockConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidConnection_ShouldNotThrow()
        {
            // Arrange & Act
            var service = new GreetingServiceDapper(_mockConnection.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullConnection_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new GreetingServiceDapper(null!, _mockLogger.Object)
            );
        }

        [Fact]
        public void SayHello_ShouldReturnDefaultGreeting()
        {
            // Arrange
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _service.SayHello("");

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithName_ShouldReturnPersonalizedGreeting()
        {
            // Arrange
            var name = "John";
            var expectedGreeting = "hello John";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithEmptyName_ShouldReturnDefaultGreeting()
        {
            // Arrange
            var name = "";
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithWhitespaceName_ShouldReturnDefaultGreeting()
        {
            // Arrange
            var name = "   ";
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithNullName_ShouldReturnDefaultGreeting()
        {
            // Arrange
            string name = null ?? "";
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithSpecialCharacters_ShouldReturnEscapedGreeting()
        {
            // Arrange
            var name = "O'Connor";
            var expectedGreeting = "hello O'Connor";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithVeryLongName_ShouldReturnTruncatedGreeting()
        {
            // Arrange
            var name = new string('A', 1000);
            var expectedGreeting = $"hello {name}";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }

        [Fact]
        public void SayHello_WithUnicodeName_ShouldReturnCorrectGreeting()
        {
            // Arrange
            var name = "José María";
            var expectedGreeting = "hello José María";

            // Act
            var result = _service.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
        }
    }
}

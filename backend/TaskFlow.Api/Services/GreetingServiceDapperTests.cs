using System;
using System.Data;
using Dapper;
using Moq;
using TaskFlow.Api.Services;
using Xunit;

namespace TaskFlow.Api.Tests.Services
{
    public class GreetingServiceDapperTests
    {
        private readonly Mock<IDbConnection> _mockDbConnection;
        private readonly Mock<ILogger<GreetingServiceDapper>> _mockLogger;
        private readonly GreetingServiceDapper _greetingService;

        public GreetingServiceDapperTests()
        {
            _mockDbConnection = new Mock<IDbConnection>();
            _mockLogger = new Mock<ILogger<GreetingServiceDapper>>();
            _greetingService = new GreetingServiceDapper(
                _mockDbConnection.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void SayHello_WithValidName_ReturnsExpectedGreeting()
        {
            // Arrange
            var name = "John";
            var expectedGreeting = "hello John";

            // Act
            var result = _greetingService.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }

        [Fact]
        public void SayHello_WithEmptyName_ReturnsAnonymousGreeting()
        {
            // Arrange
            var name = "";
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _greetingService.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }

        [Fact]
        public void SayHello_WithWhitespaceName_ReturnsAnonymousGreeting()
        {
            // Arrange
            var name = "   ";
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _greetingService.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }

        [Fact]
        public void SayHello_WithNullName_ReturnsAnonymousGreeting()
        {
            // Arrange
            string name = null!;
            var expectedGreeting = "hello anonymous";

            // Act
            var result = _greetingService.SayHello(name ?? "");

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }

        [Fact]
        public void SayHello_WithCustomGreetingFromDatabase_ReturnsCustomGreeting()
        {
            // Arrange
            var name = "Alice";
            var customGreeting = "Welcome back, Alice!";
            var expectedGreeting = customGreeting;

            // Mock Dapper query result - simplified approach
            _mockDbConnection.Setup(x => x.State).Returns(ConnectionState.Broken);

            // Act
            var result = _greetingService.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
            _mockLogger.Verify(
                x =>
                    x.LogInformation(
                        "Found custom greeting for {Name}: {Greeting}",
                        name,
                        customGreeting
                    ),
                Times.Once
            );
        }

        [Fact]
        public void SayHello_WithDatabaseException_FallsBackToDefaultGreeting()
        {
            // Arrange
            var name = "Bob";
            var expectedGreeting = "hello Bob";

            // Mock Dapper query to throw exception
            _mockDbConnection.Setup(x => x.State).Returns(ConnectionState.Broken);

            // Act
            var result = _greetingService.SayHello(name);

            // Assert
            Assert.Equal(expectedGreeting, result);
            _mockLogger.Verify(
                x =>
                    x.LogWarning(
                        It.IsAny<Exception>(),
                        "Could not retrieve custom greeting from database for {Name}",
                        name
                    ),
                Times.Once
            );
        }

        [Fact]
        public void SayHelloWithLogger_WithValidName_ReturnsExpectedGreeting()
        {
            // Arrange
            var name = "Charlie";
            var expectedGreeting = "hello Charlie";
            var mockLogger = new Mock<ILogger<GreetingServiceDapper>>();

            // Act
            var result = _greetingService.SayHelloWithLogger(name, mockLogger.Object);

            // Assert
            Assert.Equal(expectedGreeting, result);
            mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }

        [Fact]
        public void SayHelloWithLogger_WithEmptyName_ReturnsAnonymousGreeting()
        {
            // Arrange
            var name = "";
            var expectedGreeting = "hello anonymous";
            var mockLogger = new Mock<ILogger<GreetingServiceDapper>>();

            // Act
            var result = _greetingService.SayHelloWithLogger(name, mockLogger.Object);

            // Assert
            Assert.Equal(expectedGreeting, result);
            mockLogger.Verify(
                x => x.LogInformation("Greeting requested for name: {Name}", name),
                Times.Once
            );
        }
    }
}

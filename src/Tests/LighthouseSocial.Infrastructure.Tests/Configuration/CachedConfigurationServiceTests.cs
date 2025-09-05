using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Infrastructure.Caching;
using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace LighthouseSocial.Infrastructure.Tests.Configuration
{
    public class CachedConfigurationServiceTests
    {
        private readonly Mock<ISecretManager> _mockSecretManager;
        private readonly Mock<ICacheService> _cacheService;
        private readonly Mock<ILogger<CachedConfigurationService>> _mockLogger;

        public CachedConfigurationServiceTests()
        {
            _mockSecretManager = new Mock<ISecretManager>();
            _cacheService = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<CachedConfigurationService>>();
        }

        [Fact]
        public async Task GetDatabaseConnectionStringAsync_WhenSecretExists_ReturnsConnectionString()
        {
            // Arrange
            const string expectedConnectionString = "Server=localhost;Database=test;User Id=user;Password=pass;";
            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "DbConnStr"))
                .ReturnsAsync(Result<string?>.Ok(expectedConnectionString));

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act
            var result = await service.GetDatabaseConnectionStringAsync();

            // Assert
            Assert.Equal(expectedConnectionString, result);
        }

        [Fact]
        public async Task GetMinioCredentialsAsync_ReturnsCredentials()
        {
            // Arrange
            const string expectedAccessKey = "minioaccess";
            const string expectedSecretKey = "miniosecret";

            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"))
                .ReturnsAsync(Result<string?>.Ok(expectedAccessKey));

            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOSecretKey"))
                .ReturnsAsync(Result<string?>.Ok(expectedSecretKey));

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act
            var (AccessKey, SecretKey) = await service.GetMinioCredentialsAsync();

            // Assert
            Assert.Equal(expectedAccessKey, AccessKey);
            Assert.Equal(expectedSecretKey, SecretKey);

            _mockSecretManager.Verify(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"), Times.Once);
        }
    }
}

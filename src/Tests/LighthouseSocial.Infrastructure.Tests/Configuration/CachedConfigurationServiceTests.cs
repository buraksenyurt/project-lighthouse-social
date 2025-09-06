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
            var result = await service.GetMinioCredentialsAsync();

            // Assert
            Assert.Equal(expectedAccessKey, result.AccessKey);
            Assert.Equal(expectedSecretKey, result.SecretKey);

            _mockSecretManager.Verify(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"), Times.Once);
        }

        [Fact]
        public async Task GetKeycloakSettingsAsync_ReturnsSettings()
        {
            // Arrange
            SetupKeycloakSecrets();

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act
            var result = await service.GetKeycloakSettingsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-audience", result.Audience);
            Assert.Equal("https://keycloak.test.com", result.Authority);
            Assert.Equal("test-client", result.ClientId);
            Assert.Equal("test-secret", result.ClientSecret);
            Assert.Equal("test-realm", result.Realm);
        }

        private void SetupKeycloakSecrets()
        {
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakAudience"))
                .ReturnsAsync(Result<string?>.Ok("test-audience"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakAuthority"))
                .ReturnsAsync(Result<string?>.Ok("https://keycloak.test.com"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakClientId"))
                .ReturnsAsync(Result<string?>.Ok("test-client"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakClientSecret"))
                .ReturnsAsync(Result<string?>.Ok("test-secret"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakRealm"))
                .ReturnsAsync(Result<string?>.Ok("test-realm"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakClockSkew"))
                .ReturnsAsync(Result<string?>.Ok("5"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakRequireHttpsMetadata"))
                .ReturnsAsync(Result<string?>.Ok("true"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakValidateAudience"))
                .ReturnsAsync(Result<string?>.Ok("true"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakValidateIssuer"))
                .ReturnsAsync(Result<string?>.Ok("true"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakValidateIssuerSigningKey"))
                .ReturnsAsync(Result<string?>.Ok("true"));
            _mockSecretManager.Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "KeycloakValidateLifetime"))
                .ReturnsAsync(Result<string?>.Ok("true"));
        }

        [Fact]
        public async Task GetMinioCredentialsAsync_WhenAccessKeyFails_ThrowsException()
        {
            // Arrange
            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"))
                .ReturnsAsync(Result<string?>.Fail("AccessKey not found"));

            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOSecretKey"))
                .ReturnsAsync(Result<string?>.Ok("secret"));

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetMinioCredentialsAsync());
        }

        [Fact]
        public async Task GetMinioCredentialsAsync_WhenSecretKeyFails_ThrowsException()
        {
            // Arrange
            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"))
                .ReturnsAsync(Result<string?>.Ok("access"));

            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOSecretKey"))
                .ReturnsAsync(Result<string?>.Fail("SecretKey not found"));

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetMinioCredentialsAsync());
        }

        [Fact]
        public async Task GetMinioCredentialsAsync_WhenAccessKeyIsEmpty_ThrowsException()
        {
            // Arrange
            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOAccessKey"))
                .ReturnsAsync(Result<string?>.Ok(""));

            _mockSecretManager
                .Setup(x => x.GetSecretAsync("ProjectLighthouseSocial-Dev", "MinIOSecretKey"))
                .ReturnsAsync(Result<string?>.Ok("secret"));

            var service = new CachedConfigurationService(
                _mockSecretManager.Object,
                _cacheService.Object,
                _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetMinioCredentialsAsync());
        }
    }
}

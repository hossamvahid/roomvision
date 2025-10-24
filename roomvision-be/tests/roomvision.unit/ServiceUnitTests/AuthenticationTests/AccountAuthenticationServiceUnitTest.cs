using Moq;
using log4net;
using roomvision.application.Common;
using roomvision.application.Servicies;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using Xunit;
using roomvision.domain.Interfaces.Security;

namespace roomvision.unit.ServiceUnitTests
{
    public class AccountAuthenticationServiceUnitTest
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<ITokenGenerator> _mockTokenGenerator;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ILog> _mockLog;
        private readonly AuthenticationService _authService;

        public AccountAuthenticationServiceUnitTest()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockLog = new Mock<ILog>();

            _authService = new AuthenticationService(
                _mockAccountRepository.Object,
                _mockRoomRepository.Object,
                _mockTokenGenerator.Object,
                _mockPasswordHasher.Object,
                _mockLog.Object);
        }

        [Fact]
        public async Task UserAuthenticate_ReturnsSuccess_WhenCredentialsAreValid()
        {
            var email = "test@example.com";
            var plainPassword = "testpassword";
            var hashedPassword = "$2a$14$fVdR3ZkfaNp77Wa0oG1cYOcKZPQawbLAkfPGvvLDSu2rSklMdxmoi";
            var expectedToken = "jwt-token-123";

            var account = new Account
            {
                Id = 1,
                Email = email,
                Password = hashedPassword,
                Name = "Test User"
            };

            _mockAccountRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(account);

            _mockTokenGenerator.Setup(t => t.GenerateUserToken(account))
                .Returns(expectedToken);
            
            _mockPasswordHasher.Setup(p => p.VerifyHashedPassword(plainPassword, hashedPassword))
                .Returns(true);


            var result = await _authService.UserAuthenticateAsync(email, plainPassword);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedToken, result.Value);

        }

        [Fact]
        public async Task UserAuthenticate_ReturnsFailure_WhenAccountNotFound()
        {
            var email = "nonexistent@example.com";
            var password = "anyPassword";

            _mockAccountRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((Account?)null);

          
            var result = await _authService.UserAuthenticateAsync(email, password);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
            
        }

        [Fact]
        public async Task UserAuthenticate_ReturnsFailure_WhenPasswordIsIncorrect()
        {
            var email = "test@example.com";
            var wrongPassword = "wrongPassword";
            var hashedPassword = "$2a$14$fVdR3ZkfaNp77Wa0oG1cYOcKZPQawbLAkfPGvvLDSu2rSklMdxmoi";

            var account = new Account
            {
                Id = 1,
                Email = email,
                Password = hashedPassword,
                Name = "Test User"
            };

            _mockAccountRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(account);

            _mockPasswordHasher.Setup(p => p.VerifyHashedPassword(wrongPassword, hashedPassword))
                .Returns(false);

            var result = await _authService.UserAuthenticateAsync(email, wrongPassword);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
        }
    }
}
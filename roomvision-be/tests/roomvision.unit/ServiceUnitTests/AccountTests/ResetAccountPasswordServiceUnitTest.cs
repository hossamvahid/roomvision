using Moq;
using log4net;
using roomvision.application.Common;
using roomvision.application.Servicies.AccountServices;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using roomvision.domain.Interfaces.Security;

namespace roomvision.unit.ServiceUnitTests.AccountTests
{
    public class ResetAccountPasswordServiceUnitTest
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ILog> _mockLog;
        private readonly ResetAccountPasswordService _resetPasswordService;

        public ResetAccountPasswordServiceUnitTest()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockLog = new Mock<ILog>();

            _resetPasswordService = new ResetAccountPasswordService(
                _mockAccountRepository.Object,
                _mockPasswordHasher.Object,
                _mockLog.Object);
        }

        [Fact]
        public async Task Execute_ReturnsSuccess_WhenPasswordResetSuccessfully()
        {
            var accountId = 123;
            var newPassword = "NewSecurePassword123";
            var hashedPassword = "$2b$10$eImiTXuWVxfM37uY4JANjOHJNRSaEs7yvPklS1ZzXegEKRmOSt2OG";

            var existingAccount = new Account
            {
                Id = accountId,
                Email = "user@example.com",
                Name = "Test User",
                Password = "oldHashedPassword"
            };

            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync(existingAccount);

            _mockPasswordHasher.Setup(p => p.GenerateHashedPassword(newPassword))
                .Returns(hashedPassword);

            var result = await _resetPasswordService.Execute(accountId, newPassword);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            var accountId = 999;
            var newPassword = "NewSecurePassword123";

            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync((Account?)null);

            var result = await _resetPasswordService.Execute(accountId, newPassword);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
        }
    }
}
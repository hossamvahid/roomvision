using Moq;
using log4net;
using roomvision.application.Common;
using roomvision.application.Servicies.AccountServices;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using Xunit;
using roomvision.domain.Interfaces.Security;

namespace roomvision.unit.ServiceUnitTests.AccountTests
{
    public class CreateAccountServiceUnitTest
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<ILog> _mockLog;
        private readonly Mock<IMailGenerator> _mockMailGenerator;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly CreateAccountService _createAccountService;

        public CreateAccountServiceUnitTest()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockLog = new Mock<ILog>();
            _mockMailGenerator = new Mock<IMailGenerator>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();

            _createAccountService = new CreateAccountService(
                _mockAccountRepository.Object,
                _mockLog.Object,
                _mockMailGenerator.Object,
                _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task Execute_ReturnsSuccess_WhenAccountCreatedSuccessfully()
        {
            var email = "newuser@example.com";
            var name = "New User";
            var hashedPassword = "$2b$10$eImiTXuWVxfM37uY4JANjOHJNRSaEs7yvPklS1ZzXegEKRmOSt2OG";

            _mockAccountRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((Account?)null);



            _mockPasswordHasher.Setup(p => p.GenerateHashedPassword(It.IsAny<string>()))
                .Returns(hashedPassword);
            

            _mockMailGenerator.Setup(m => m.WelcomeEmailAsync(email, It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            var result = await _createAccountService.Execute(email, name);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_ReturnsConflict_WhenAccountAlreadyExists()
        {
            var email = "existing@example.com";
            var name = "Existing User";

            var existingAccount = new Account
            {
                Id = 1,
                Email = email,
                Name = "Existing Account",
                Password = "hashedPassword"
            };

            _mockAccountRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(existingAccount);

            var result = await _createAccountService.Execute(email, name);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.Conflict, result.ErrorType);
        }
    }
}
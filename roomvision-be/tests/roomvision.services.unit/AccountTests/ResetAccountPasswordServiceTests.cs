using roomvision.application.Servicies.AccountServices;

namespace roomvision.services.unit.AccountTests;

public class ResetAccountPasswordServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ILog> _log = new();
    private readonly ResetAccountPasswordService _sut;

    public ResetAccountPasswordServiceTests()
    {
        _sut = new ResetAccountPasswordService(
            _accountRepo.Object,
            _passwordHasher.Object,
            _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenAccountExistsAndPasswordUpdated()
    {
        var account = new Account { Id = 1, Name = "Alice", Password = "old-hash" };
        _accountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);
        _passwordHasher.Setup(p => p.GenerateHashedPassword("newPass")).Returns("new-hash");

        var result = await _sut.Execute(1, "newPass");

        Assert.True(result.IsSuccess);
        Assert.Equal("new-hash", account.Password);
        _accountRepo.Verify(r => r.UpdateAsync(account), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        _accountRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Account?)null);

        var result = await _sut.Execute(99, "anything");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
        _accountRepo.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task Execute_HashesNewPasswordBeforeSaving()
    {
        var account = new Account { Id = 2, Password = "old" };
        _accountRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(account);
        _passwordHasher.Setup(p => p.GenerateHashedPassword("secret123")).Returns("hashed-secret");

        await _sut.Execute(2, "secret123");

        _passwordHasher.Verify(p => p.GenerateHashedPassword("secret123"), Times.Once);
        Assert.Equal("hashed-secret", account.Password);
    }
}

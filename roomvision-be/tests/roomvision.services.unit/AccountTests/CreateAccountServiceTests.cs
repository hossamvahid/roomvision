using roomvision.application.Servicies.AccountServices;

namespace roomvision.services.unit.AccountTests;

public class CreateAccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IMailGenerator> _mailGenerator = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ILog> _log = new();
    private readonly CreateAccountService _sut;

    public CreateAccountServiceTests()
    {
        _sut = new CreateAccountService(
            _accountRepo.Object,
            _log.Object,
            _mailGenerator.Object,
            _passwordHasher.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenAccountIsNew()
    {
        _accountRepo.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((Account?)null);
        _passwordHasher.Setup(p => p.GenerateHashedPassword(It.IsAny<string>())).Returns("hashed");
        _mailGenerator.Setup(m => m.WelcomeEmailAsync("new@test.com", It.IsAny<string>())).Returns(Task.CompletedTask);

        var result = await _sut.Execute("new@test.com", "Alice", Role.Viewer);

        Assert.True(result.IsSuccess);
        _accountRepo.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
        _mailGenerator.Verify(m => m.WelcomeEmailAsync("new@test.com", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsConflict_WhenEmailAlreadyExists()
    {
        var existing = new Account { Id = 1, Email = "dup@test.com", Name = "Dup" };
        _accountRepo.Setup(r => r.GetByEmailAsync("dup@test.com")).ReturnsAsync(existing);

        var result = await _sut.Execute("dup@test.com", "Dup", Role.Viewer);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Conflict, result.ErrorType);
        _accountRepo.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task Execute_HashesPasswordBeforeSaving()
    {
        _accountRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);
        _passwordHasher.Setup(p => p.GenerateHashedPassword(It.IsAny<string>())).Returns("hashed-pw");
        _mailGenerator.Setup(m => m.WelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        Account? saved = null;
        _accountRepo.Setup(r => r.AddAsync(It.IsAny<Account>()))
            .Callback<Account>(a => saved = a);

        await _sut.Execute("any@test.com", "Bob", Role.Admin);

        Assert.Equal("hashed-pw", saved?.Password);
    }

    [Fact]
    public async Task Execute_AssignsCorrectRole()
    {
        _accountRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);
        _passwordHasher.Setup(p => p.GenerateHashedPassword(It.IsAny<string>())).Returns("h");
        _mailGenerator.Setup(m => m.WelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        Account? saved = null;
        _accountRepo.Setup(r => r.AddAsync(It.IsAny<Account>()))
            .Callback<Account>(a => saved = a);

        await _sut.Execute("admin@test.com", "Admin", Role.Admin);

        Assert.Equal(Role.Admin, saved?.Role);
    }
}

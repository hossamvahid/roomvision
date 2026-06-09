using roomvision.application.Servicies.AccountServices;

namespace roomvision.services.unit.AccountTests;

public class GetAccountNameServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly GetAccountNameService _sut;

    public GetAccountNameServiceTests()
    {
        _sut = new GetAccountNameService(_accountRepo.Object);
    }

    [Fact]
    public async Task Execute_ReturnsAccountName_WhenAccountExists()
    {
        var account = new Account { Id = 3, Name = "Jane Doe" };
        _accountRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(account);

        var result = await _sut.Execute(3);

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane Doe", result.Value);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        _accountRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Account?)null);

        var result = await _sut.Execute(42);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }
}

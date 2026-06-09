using roomvision.application.Servicies.AccountServices;

namespace roomvision.services.unit.AccountTests;

public class DeleteAccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<ILog> _log = new();
    private readonly DeleteAccountService _sut;

    public DeleteAccountServiceTests()
    {
        _sut = new DeleteAccountService(_accountRepo.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenViewerAccountDeleted()
    {
        var account = new Account { Id = 5, Name = "Viewer", Role = Role.Viewer };
        _accountRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(account);

        var result = await _sut.Execute(5);

        Assert.True(result.IsSuccess);
        _accountRepo.Verify(r => r.DeleteByIdAsync(account), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        _accountRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Account?)null);

        var result = await _sut.Execute(99);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
        _accountRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsValidationError_WhenDeletingAdminAccount()
    {
        var admin = new Account { Id = 1, Name = "Admin", Role = Role.Admin };
        _accountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(admin);

        var result = await _sut.Execute(1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Validation, result.ErrorType);
        _accountRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Account>()), Times.Never);
    }
}

using roomvision.application.Servicies.AccountServices;

namespace roomvision.services.unit.AccountTests;

public class GetAccountsServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IProjection<Account, AccountDetails>> _projection = new();
    private readonly Mock<ILog> _log = new();
    private readonly GetAccountsService _sut;

    public GetAccountsServiceTests()
    {
        _sut = new GetAccountsService(_accountRepo.Object, _projection.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsPagedAccounts_WhenCalled()
    {
        var accounts = new List<Account>
        {
            new() { Id = 1, Name = "Alice", Email = "a@test.com" },
            new() { Id = 2, Name = "Bob",   Email = "b@test.com" },
        };
        var details = new List<AccountDetails>
        {
            new() { Id = 1, Name = "Alice", Email = "a@test.com" },
            new() { Id = 2, Name = "Bob",   Email = "b@test.com" },
        };

        _accountRepo.Setup(r => r.GetAccountsPaginatedAsync(1, 6)).ReturnsAsync(accounts);
        _accountRepo.Setup(r => r.CountAsync()).ReturnsAsync(2);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Account>>()))
            .ReturnsAsync(details);

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.items!.Count);
        Assert.Equal(1, result.Value.totalPages);
    }

    [Fact]
    public async Task Execute_ExcludesSpecifiedAccount_WhenExcludeIdProvided()
    {
        var accounts = new List<Account>
        {
            new() { Id = 1, Name = "Alice", Email = "a@test.com" },
            new() { Id = 2, Name = "Bob",   Email = "b@test.com" },
        };
        var filteredDetails = new List<AccountDetails>
        {
            new() { Id = 2, Name = "Bob", Email = "b@test.com" },
        };

        _accountRepo.Setup(r => r.GetAccountsPaginatedAsync(1, 6)).ReturnsAsync(accounts);
        _accountRepo.Setup(r => r.CountAsync()).ReturnsAsync(2);
        _projection.Setup(p => p.ProjectAsync(It.Is<IReadOnlyList<Account>>(list =>
                list.Count == 1 && list[0].Id == 2)))
            .ReturnsAsync(filteredDetails);

        var result = await _sut.Execute(1, excludeAccountId: 1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.items!);
        Assert.Equal(2, result.Value.items![0].Id);
    }

    [Fact]
    public async Task Execute_ReturnsEmptyPage_WhenNoAccountsExist()
    {
        _accountRepo.Setup(r => r.GetAccountsPaginatedAsync(1, 6)).ReturnsAsync(new List<Account>());
        _accountRepo.Setup(r => r.CountAsync()).ReturnsAsync(0);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Account>>()))
            .ReturnsAsync(new List<AccountDetails>());

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.items!);
        Assert.Equal(0, result.Value.totalPages);
    }

    [Fact]
    public async Task Execute_CalculatesCorrectTotalPages()
    {
        var accounts = Enumerable.Range(1, 6)
            .Select(i => new Account { Id = i, Name = $"User{i}", Email = $"u{i}@test.com" })
            .ToList();
        var details = accounts
            .Select(a => new AccountDetails { Id = a.Id, Name = a.Name!, Email = a.Email! })
            .ToList();

        _accountRepo.Setup(r => r.GetAccountsPaginatedAsync(1, 6)).ReturnsAsync(accounts);
        _accountRepo.Setup(r => r.CountAsync()).ReturnsAsync(13);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Account>>())).ReturnsAsync(details);

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.totalPages); // ceil(13/6) = 3
    }
}

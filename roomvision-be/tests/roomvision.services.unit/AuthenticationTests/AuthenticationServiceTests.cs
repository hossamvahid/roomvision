using roomvision.application.Servicies;

namespace roomvision.services.unit.AuthenticationTests;

public class AuthenticationServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ILog> _log = new();
    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        _sut = new AuthenticationService(
            _accountRepo.Object,
            _roomRepo.Object,
            _tokenGenerator.Object,
            _passwordHasher.Object,
            _log.Object);
    }

    // ── UserAuthenticateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UserAuthenticate_ReturnsSuccess_WhenCredentialsAreValid()
    {
        var account = new Account { Id = 1, Email = "user@test.com", Password = "hashed", Name = "User" };
        _accountRepo.Setup(r => r.GetByEmailAsync(account.Email!)).ReturnsAsync(account);
        _passwordHasher.Setup(p => p.VerifyHashedPassword("plain", "hashed")).Returns(true);
        _tokenGenerator.Setup(t => t.GenerateUserToken(account)).Returns("jwt-token");

        var result = await _sut.UserAuthenticateAsync(account.Email!, "plain");

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value);
    }

    [Fact]
    public async Task UserAuthenticate_ReturnsUnauthorized_WhenAccountNotFound()
    {
        _accountRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);

        var result = await _sut.UserAuthenticateAsync("ghost@test.com", "any");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task UserAuthenticate_ReturnsUnauthorized_WhenPasswordIsWrong()
    {
        var account = new Account { Id = 1, Email = "user@test.com", Password = "hashed", Name = "User" };
        _accountRepo.Setup(r => r.GetByEmailAsync(account.Email!)).ReturnsAsync(account);
        _passwordHasher.Setup(p => p.VerifyHashedPassword("wrong", "hashed")).Returns(false);

        var result = await _sut.UserAuthenticateAsync(account.Email!, "wrong");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task UserAuthenticate_ReturnsUnauthorized_WhenPasswordHashIsEmpty()
    {
        var account = new Account { Id = 1, Email = "user@test.com", Password = "", Name = "User" };
        _accountRepo.Setup(r => r.GetByEmailAsync(account.Email!)).ReturnsAsync(account);

        var result = await _sut.UserAuthenticateAsync(account.Email!, "any");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }

    // ── RoomAuthenticateAsync ─────────────────────────────────────────────

    [Fact]
    public async Task RoomAuthenticate_ReturnsSuccess_WhenCredentialsAreValid()
    {
        var room = new Room { Id = 1, RoomName = "Lab", Password = "hashed" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab")).ReturnsAsync(room);
        _passwordHasher.Setup(p => p.VerifyHashedPassword("plain", "hashed")).Returns(true);
        _tokenGenerator.Setup(t => t.GenerateRoomToken(room)).Returns("room-jwt");

        var result = await _sut.RoomAuthenticateAsync("Lab", "plain");

        Assert.True(result.IsSuccess);
        Assert.Equal("room-jwt", result.Value);
    }

    [Fact]
    public async Task RoomAuthenticate_ReturnsUnauthorized_WhenRoomNotFound()
    {
        _roomRepo.Setup(r => r.GetByRoomNameAsync(It.IsAny<string>())).ReturnsAsync((Room?)null);

        var result = await _sut.RoomAuthenticateAsync("Ghost", "any");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task RoomAuthenticate_ReturnsUnauthorized_WhenPasswordIsWrong()
    {
        var room = new Room { Id = 1, RoomName = "Lab", Password = "hashed" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab")).ReturnsAsync(room);
        _passwordHasher.Setup(p => p.VerifyHashedPassword("wrong", "hashed")).Returns(false);

        var result = await _sut.RoomAuthenticateAsync("Lab", "wrong");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task RoomAuthenticate_ReturnsUnauthorized_WhenRoomPasswordHashIsEmpty()
    {
        var room = new Room { Id = 1, RoomName = "Lab", Password = "" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab")).ReturnsAsync(room);

        var result = await _sut.RoomAuthenticateAsync("Lab", "any");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
    }
}

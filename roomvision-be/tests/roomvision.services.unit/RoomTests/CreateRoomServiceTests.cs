using roomvision.application.Servicies.RoomServices;

namespace roomvision.services.unit.RoomTests;

public class CreateRoomServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ILog> _log = new();
    private readonly CreateRoomService _sut;

    public CreateRoomServiceTests()
    {
        _sut = new CreateRoomService(_roomRepo.Object, _passwordHasher.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenRoomNameIsUnique()
    {
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync((Room?)null);
        _passwordHasher.Setup(p => p.GenerateHashedPassword("pass")).Returns("hashed");

        var result = await _sut.Execute("Lab A", "pass");

        Assert.True(result.IsSuccess);
        _roomRepo.Verify(r => r.AddAsync(It.Is<Room>(rm => rm.RoomName == "Lab A" && rm.Password == "hashed")), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsConflict_WhenRoomAlreadyExists()
    {
        var existing = new Room { Id = 1, RoomName = "Lab A" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync(existing);

        var result = await _sut.Execute("Lab A", "pass");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Conflict, result.ErrorType);
        _roomRepo.Verify(r => r.AddAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task Execute_HashesPasswordBeforeSaving()
    {
        _roomRepo.Setup(r => r.GetByRoomNameAsync(It.IsAny<string>())).ReturnsAsync((Room?)null);
        _passwordHasher.Setup(p => p.GenerateHashedPassword("raw")).Returns("hashed-raw");

        Room? saved = null;
        _roomRepo.Setup(r => r.AddAsync(It.IsAny<Room>()))
            .Callback<Room>(r => saved = r);

        await _sut.Execute("Room X", "raw");

        Assert.Equal("hashed-raw", saved?.Password);
    }
}

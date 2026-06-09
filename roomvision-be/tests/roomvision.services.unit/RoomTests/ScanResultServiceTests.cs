using roomvision.application.Servicies.RoomServices;

namespace roomvision.services.unit.RoomTests;

public class ScanResultServiceTests
{
    private readonly Mock<IScannedRoomRepository> _scannedRoomRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<ILog> _log = new();
    private readonly ScanResultService _sut;

    public ScanResultServiceTests()
    {
        _sut = new ScanResultService(_scannedRoomRepo.Object, _roomRepo.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsScannedRoom_WhenRoomExistsAndHasScan()
    {
        var room = new Room { Id = 1, RoomName = "Lab A" };
        var scan = new ScannedRoom
        {
            RoomName = "Lab A",
            TotalFaces = 4,
            IdentifiedFaces = new List<string> { "Alice", "Bob" },
            ScannedAt = DateTime.UtcNow,
        };

        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync(room);
        _scannedRoomRepo.Setup(r => r.GetScannedRoomResult("Lab A")).ReturnsAsync(scan);

        var result = await _sut.Execute("Lab A");

        Assert.True(result.IsSuccess);
        Assert.Equal("Lab A", result.Value!.RoomName);
        Assert.Equal(4, result.Value.TotalFaces);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenRoomDoesNotExist()
    {
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Ghost")).ReturnsAsync((Room?)null);

        var result = await _sut.Execute("Ghost");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
        _scannedRoomRepo.Verify(r => r.GetScannedRoomResult(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenNoScanExistsForRoom()
    {
        var room = new Room { Id = 2, RoomName = "Lab B" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab B")).ReturnsAsync(room);
        _scannedRoomRepo.Setup(r => r.GetScannedRoomResult("Lab B")).ReturnsAsync((ScannedRoom?)null);

        var result = await _sut.Execute("Lab B");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }
}

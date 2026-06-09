using roomvision.application.Servicies.RoomServices;

namespace roomvision.services.unit.RoomTests;

public class GetRoomsServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<IProjection<Room, RoomWithScan>> _projection = new();
    private readonly Mock<ILog> _log = new();
    private readonly GetRooms _sut;

    public GetRoomsServiceTests()
    {
        _sut = new GetRooms(_roomRepo.Object, _projection.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsPagedRooms_WhenRoomsExist()
    {
        var rooms = new List<Room>
        {
            new() { Id = 1, RoomName = "Lab A" },
            new() { Id = 2, RoomName = "Lab B" },
        };
        var roomScans = new List<RoomWithScan>
        {
            new() { RoomName = "Lab A", TotalFaces = 3 },
            new() { RoomName = "Lab B", TotalFaces = 5 },
        };

        _roomRepo.Setup(r => r.GetPagedAsync(1, 6)).ReturnsAsync(rooms);
        _roomRepo.Setup(r => r.CountAsync()).ReturnsAsync(2);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Room>>())).ReturnsAsync(roomScans);

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.items!.Count);
        Assert.Equal(1, result.Value.totalPages);
    }

    [Fact]
    public async Task Execute_ReturnsEmptyPage_WhenNoRoomsExist()
    {
        _roomRepo.Setup(r => r.GetPagedAsync(1, 6)).ReturnsAsync(new List<Room>());
        _roomRepo.Setup(r => r.CountAsync()).ReturnsAsync(0);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Room>>())).ReturnsAsync(new List<RoomWithScan>());

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.items!);
        Assert.Equal(0, result.Value.totalPages);
    }

    [Fact]
    public async Task Execute_CalculatesCorrectTotalPages()
    {
        var rooms = Enumerable.Range(1, 6).Select(i => new Room { Id = i, RoomName = $"Room {i}" }).ToList();
        var scans = rooms.Select(r => new RoomWithScan { RoomName = r.RoomName }).ToList();

        _roomRepo.Setup(r => r.GetPagedAsync(1, 6)).ReturnsAsync(rooms);
        _roomRepo.Setup(r => r.CountAsync()).ReturnsAsync(7);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Room>>())).ReturnsAsync(scans);

        var result = await _sut.Execute(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.totalPages); // ceil(7/6) = 2
    }

    [Fact]
    public async Task Execute_UsesCustomPageSize_WhenProvided()
    {
        _roomRepo.Setup(r => r.GetPagedAsync(1, 3)).ReturnsAsync(new List<Room>());
        _roomRepo.Setup(r => r.CountAsync()).ReturnsAsync(0);
        _projection.Setup(p => p.ProjectAsync(It.IsAny<IReadOnlyList<Room>>())).ReturnsAsync(new List<RoomWithScan>());

        await _sut.Execute(1, pageSize: 3);

        _roomRepo.Verify(r => r.GetPagedAsync(1, 3), Times.Once);
    }
}

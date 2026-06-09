using roomvision.application.Servicies.RoomServices;

namespace roomvision.services.unit.RoomTests;

public class ScanRoomServiceTests
{
    private readonly Mock<IFaceRecognition> _faceRecognition = new();
    private readonly Mock<IScannedRoomRepository> _scannedRoomRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<ILog> _log = new();
    private readonly ScanRoomService _sut;

    public ScanRoomServiceTests()
    {
        _sut = new ScanRoomService(
            _faceRecognition.Object,
            _scannedRoomRepo.Object,
            _roomRepo.Object,
            _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenScanSucceeds()
    {
        var room = new Room { Id = 1, RoomName = "Lab A" };
        var scanResult = new RoomScanResult(new List<string> { "Alice", "Bob" }, TotalFaces: 2);
        var imageBytes = new byte[] { 1, 2, 3 };

        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync(room);
        _faceRecognition.Setup(f => f.VerifyRoomAsync(imageBytes))
            .ReturnsAsync(Result<RoomScanResult>.Success(scanResult));

        var result = await _sut.Execute(imageBytes, "Lab A");

        Assert.True(result.IsSuccess);
        _scannedRoomRepo.Verify(r => r.AddScannedRoomResult(It.Is<ScannedRoom>(s =>
            s.RoomName == "Lab A" && s.TotalFaces == 2 && s.IdentifiedFaces!.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsNotFound_WhenRoomDoesNotExist()
    {
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Ghost")).ReturnsAsync((Room?)null);

        var result = await _sut.Execute(Array.Empty<byte>(), "Ghost");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
        _faceRecognition.Verify(f => f.VerifyRoomAsync(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsFailure_WhenFaceRecognitionFails()
    {
        var room = new Room { Id = 1, RoomName = "Lab A" };
        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync(room);
        _faceRecognition.Setup(f => f.VerifyRoomAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result<RoomScanResult>.Failure("Recognition error", ErrorTypes.Validation));

        var result = await _sut.Execute(new byte[] { 1 }, "Lab A");

        Assert.False(result.IsSuccess);
        Assert.Equal("Recognition error", result.Error);
        _scannedRoomRepo.Verify(r => r.AddScannedRoomResult(It.IsAny<ScannedRoom>()), Times.Never);
    }

    [Fact]
    public async Task Execute_PersistsScannedRoomWithCorrectData()
    {
        var room = new Room { Id = 1, RoomName = "Lab A" };
        var faces = new List<string> { "Alice" };
        var scanResult = new RoomScanResult(faces, TotalFaces: 3);
        var imageBytes = new byte[] { 9, 8, 7 };

        _roomRepo.Setup(r => r.GetByRoomNameAsync("Lab A")).ReturnsAsync(room);
        _faceRecognition.Setup(f => f.VerifyRoomAsync(imageBytes))
            .ReturnsAsync(Result<RoomScanResult>.Success(scanResult));

        ScannedRoom? persisted = null;
        _scannedRoomRepo.Setup(r => r.AddScannedRoomResult(It.IsAny<ScannedRoom>()))
            .Callback<ScannedRoom>(s => persisted = s);

        await _sut.Execute(imageBytes, "Lab A");

        Assert.NotNull(persisted);
        Assert.Equal("Lab A", persisted!.RoomName);
        Assert.Equal(3, persisted.TotalFaces);
        Assert.NotNull(persisted.Id);
        Assert.True(persisted.ScannedAt <= DateTime.UtcNow);
    }
}

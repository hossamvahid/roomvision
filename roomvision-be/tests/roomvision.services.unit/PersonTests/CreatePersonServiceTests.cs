using roomvision.application.Servicies.PersonServices;

namespace roomvision.services.unit.PersonTests;

public class CreatePersonServiceTests
{
    private readonly Mock<IPersonRepository> _personRepo = new();
    private readonly Mock<IFaceRecognition> _faceRecognition = new();
    private readonly Mock<ILog> _log = new();
    private readonly CreatePersonService _sut;

    public CreatePersonServiceTests()
    {
        _sut = new CreatePersonService(_personRepo.Object, _faceRecognition.Object, _log.Object);
    }

    [Fact]
    public async Task Execute_ReturnsSuccess_WhenPersonCreatedSuccessfully()
    {
        var imageBytes = new byte[] { 1, 2, 3 };
        _personRepo.Setup(r => r.GetByNameAsync("Alice")).ReturnsAsync((Person?)null);
        _faceRecognition.Setup(f => f.EmbedFaceAsync("Alice", imageBytes))
            .ReturnsAsync(Result.Success());

        var result = await _sut.Execute("Alice", imageBytes);

        Assert.True(result.IsSuccess);
        _personRepo.Verify(r => r.AddAsync(It.Is<Person>(p => p.Name == "Alice")), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsConflict_WhenPersonAlreadyExists()
    {
        var existing = new Person { Id = "1", Name = "Alice" };
        _personRepo.Setup(r => r.GetByNameAsync("Alice")).ReturnsAsync(existing);

        var result = await _sut.Execute("Alice", new byte[] { 1 });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorTypes.Conflict, result.ErrorType);
        _faceRecognition.Verify(f => f.EmbedFaceAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        _personRepo.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public async Task Execute_ReturnsFailure_WhenFaceEmbeddingFails()
    {
        var imageBytes = new byte[] { 5, 6, 7 };
        _personRepo.Setup(r => r.GetByNameAsync("Bob")).ReturnsAsync((Person?)null);
        _faceRecognition.Setup(f => f.EmbedFaceAsync("Bob", imageBytes))
            .ReturnsAsync(Result.Failure("No face detected", ErrorTypes.Validation));

        var result = await _sut.Execute("Bob", imageBytes);

        Assert.False(result.IsSuccess);
        Assert.Equal("No face detected", result.Error);
        _personRepo.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public async Task Execute_SavesPersonWithUniqueId()
    {
        var imageBytes = new byte[] { 1 };
        _personRepo.Setup(r => r.GetByNameAsync("Carol")).ReturnsAsync((Person?)null);
        _faceRecognition.Setup(f => f.EmbedFaceAsync("Carol", imageBytes)).ReturnsAsync(Result.Success());

        Person? saved = null;
        _personRepo.Setup(r => r.AddAsync(It.IsAny<Person>()))
            .Callback<Person>(p => saved = p);

        await _sut.Execute("Carol", imageBytes);

        Assert.NotNull(saved);
        Assert.NotNull(saved!.Id);
        Assert.True(Guid.TryParse(saved.Id, out _));
        Assert.Equal("Carol", saved.Name);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), saved.CreatedAt);
    }

    [Fact]
    public async Task Execute_CallsFaceEmbedding_WithCorrectArguments()
    {
        var imageBytes = new byte[] { 10, 20, 30 };
        _personRepo.Setup(r => r.GetByNameAsync("Dave")).ReturnsAsync((Person?)null);
        _faceRecognition.Setup(f => f.EmbedFaceAsync("Dave", imageBytes)).ReturnsAsync(Result.Success());
        _personRepo.Setup(r => r.AddAsync(It.IsAny<Person>())).Returns(Task.CompletedTask);

        await _sut.Execute("Dave", imageBytes);

        _faceRecognition.Verify(f => f.EmbedFaceAsync("Dave", imageBytes), Times.Once);
    }
}

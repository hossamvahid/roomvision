using Moq;
using log4net;
using roomvision.application.Common;
using roomvision.application.Servicies;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using Xunit;

namespace roomvision.unit.ServiceUnitTests
{
    public class RoomAuthenticationServiceUnitTest
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<ITokenGenerator> _mockTokenGenerator;
        private readonly Mock<IPasswordGenerator> _mockPasswordGenerator;
        private readonly Mock<ILog> _mockLog;
        private readonly AuthenticationService _authService;

        public RoomAuthenticationServiceUnitTest()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();
            _mockPasswordGenerator = new Mock<IPasswordGenerator>();
            _mockLog = new Mock<ILog>();

            _authService = new AuthenticationService(
                _mockAccountRepository.Object,
                _mockRoomRepository.Object,
                _mockTokenGenerator.Object,
                _mockPasswordGenerator.Object,
                _mockLog.Object);
        }

        [Fact]
        public async Task RoomAuthenticate_ReturnsSuccess_WhenCredentialsAreValid()
        {
            var roomName = "ConferenceRoom";
            var plainPassword = "testpassword";
            var hashedPassword = "$2a$14$fVdR3ZkfaNp77Wa0oG1cYOcKZPQawbLAkfPGvvLDSu2rSklMdxmoi";
            var expectedToken = "jwt-room-token-456";

            var room = new Room
            {
                Id = 1,
                RoomName = roomName,
                Password = hashedPassword
            };

            _mockRoomRepository.Setup(r => r.GetByRoomNameAsync(roomName))
                .ReturnsAsync(room);

            _mockTokenGenerator.Setup(t => t.GenerateRoomToken(room))
                .Returns(expectedToken);

            var result = await _authService.RoomAuthenticateAsync(roomName, plainPassword);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedToken, result.Value);
        }

        [Fact]
        public async Task RoomAuthenticate_ReturnsFailure_WhenRoomNotFound()
        {
            var roomName = "NonexistentRoom";
            var password = "anyPassword";

            _mockRoomRepository.Setup(r => r.GetByRoomNameAsync(roomName))
                .ReturnsAsync((Room?)null);

            var result = await _authService.RoomAuthenticateAsync(roomName, password);

            
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
        }

        [Fact]
        public async Task RoomAuthenticate_ReturnsFailure_WhenPasswordIsIncorrect()
        {
            var roomName = "TestRoom";
            var wrongPassword = "wrongPassword";
            var hashedPassword = "$2a$14$fVdR3ZkfaNp77Wa0oG1cYOcKZPQawbLAkfPGvvLDSu2rSklMdxmoi";
            var room = new Room
            {
                Id = 1,
                RoomName = roomName,
                Password = hashedPassword
            };

            _mockRoomRepository.Setup(r => r.GetByRoomNameAsync(roomName))
                .ReturnsAsync(room);

            var result = await _authService.RoomAuthenticateAsync(roomName, wrongPassword);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorTypes.Unauthorized, result.ErrorType);
        }
    }
}
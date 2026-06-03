using log4net;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Repositories;
using roomvision.domain.Interfaces.Security;

namespace roomvision.application.Servicies.RoomServices
{
    public class CreateRoomService : ICreateRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILog _log;

        public CreateRoomService(IRoomRepository roomRepository, IPasswordHasher passwordHasher, ILog log)
        {
            _roomRepository = roomRepository;
            _passwordHasher = passwordHasher;
            _log = log;
        }

        public async Task<Result> Execute(string roomName, string password)
        {
            _log.Info($"Creating room with name: {roomName}");

            var existing = await _roomRepository.GetByRoomNameAsync(roomName);
            if (existing is not null)
            {
                _log.Warn($"Room with name {roomName} already exists.");
                return Result.Failure("A room with this name already exists.", ErrorTypes.Conflict);
            }

            var room = new Room
            {
                RoomName = roomName,
                Password = _passwordHasher.GenerateHashedPassword(password),
            };

            await _roomRepository.AddAsync(room);
            _log.Info($"Room '{roomName}' created successfully.");
            return Result.Success();
        }
    }
}

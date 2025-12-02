using log4net;
using roomvision.domain.Common;
using roomvision.application.Interfaces.Servicies;
using roomvision.application.Utilities;
using roomvision.domain.Interfaces.Generators;
using roomvision.domain.Interfaces.Repositories;
using roomvision.domain.Interfaces.Security;

namespace roomvision.application.Servicies
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILog _log;

        public AuthenticationService(
            IAccountRepository accountRepository,
            IRoomRepository roomRepository,
            ITokenGenerator tokenGenerator,
            IPasswordHasher passwordHasher,
            ILog log)
        {
            _accountRepository = accountRepository;
            _roomRepository = roomRepository;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
            _log = log;
        }

        public async Task<Result<string>> UserAuthenticateAsync(string email, string password)
        {
            _log.Info($"Authenticating user with email: {email}");
            var account = await _accountRepository.GetByEmailAsync(email);

            if (account is null || string.IsNullOrEmpty(account.Password) || _passwordHasher.VerifyHashedPassword(password, account.Password) is false)
            {
                _log.Warn($"Authentication failed: Incorrect password for email {email}");
                return Result<string>.Failure("Invalid email or password.", ErrorTypes.Unauthorized);
            }

            var token = _tokenGenerator.GenerateUserToken(account);
            _log.Info($"User with email {email} authenticated successfully.");
            return Result<string>.Success(token);
        }

        public async Task<Result<string>> RoomAuthenticateAsync(string roomName, string password)
        {
            _log.Info($"Authenticating room with name: {roomName}");
            var room = await _roomRepository.GetByRoomNameAsync(roomName);

            if (room is null || string.IsNullOrEmpty(room.Password) || _passwordHasher.VerifyHashedPassword(password, room.Password) is false)
            {
                _log.Warn($"Authentication failed: Incorrect password for room with name {roomName}");
                return Result<string>.Failure("Invalid room name or password.", ErrorTypes.Unauthorized);
            }

            var token = _tokenGenerator.GenerateRoomToken(room);
            _log.Info($"Room with name {roomName} authenticated successfully.");
            return Result<string>.Success(token);
        }
    }
}
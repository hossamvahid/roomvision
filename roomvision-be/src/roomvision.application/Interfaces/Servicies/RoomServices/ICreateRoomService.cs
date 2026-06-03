using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.RoomServices
{
    public interface ICreateRoomService
    {
        Task<Result> Execute(string roomName, string password);
    }
}

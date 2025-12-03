using System;
using roomvision.domain.Common;

namespace roomvision.application.Interfaces.Servicies.RoomServices;

public interface IScanRoomService
{
    public Task<Result> Execute(byte[] imageFile, string roomName);
}

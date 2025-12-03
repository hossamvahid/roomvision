using System;
using roomvision.domain.Common;
using roomvision.domain.Entities;

namespace roomvision.application.Interfaces.Servicies.RoomServices;

public interface IScanResultService
{
    public Task<Result<ScannedRoom>> Execute(string roomName);
}

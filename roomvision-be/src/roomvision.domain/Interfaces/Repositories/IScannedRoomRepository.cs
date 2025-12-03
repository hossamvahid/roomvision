using System;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories;

public interface IScannedRoomRepository
{
    public Task<ScannedRoom?> GetScannedRoomResult(string roomName);
    public Task AddScannedRoomResult(ScannedRoom scannedRoom);
}

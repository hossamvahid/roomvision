using System;
using log4net;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.RoomServices;

public class ScanResultService : IScanResultService
{
    private readonly IScannedRoomRepository _scannedRoomRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILog _log;

    public ScanResultService(IScannedRoomRepository scannedRoomRepository, IRoomRepository roomRepository, ILog log)
    {
        _scannedRoomRepository = scannedRoomRepository;
        _roomRepository = roomRepository;
        _log = log;
    }

    public async Task<Result<ScannedRoom>> Execute(string roomName)
    {
        _log.Info($"Fetching scanned room result for room: {roomName}");

        _log.Info($"Checking if room {roomName} exists.");
        var roomExists = await _roomRepository.GetByRoomNameAsync(roomName);
        if (roomExists is null)
        {
            _log.Warn($"Room with name {roomName} does not exist.");
            return Result<ScannedRoom>.Failure($"Room with name {roomName} does not exist.", ErrorTypes.NotFound);
        }

        _log.Info($"Fetching scanned result for room {roomName}.");
        var scannedRoomResult = await _scannedRoomRepository.GetScannedRoomResult(roomName);
        if (scannedRoomResult is null)
        {
            _log.Info($"No scanned result found for room {roomName}.");
            return Result<ScannedRoom>.Failure($"No scanned result found for room {roomName}.", ErrorTypes.NotFound);
        }

        _log.Info($"Scanned result found for room {roomName}.");
        return Result<ScannedRoom>.Success(scannedRoomResult);
    }

}

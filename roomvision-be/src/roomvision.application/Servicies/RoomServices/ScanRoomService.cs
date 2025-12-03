using System;
using log4net;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.FaceRecognition;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.RoomServices;

public class ScanRoomService : IScanRoomService
{
    private readonly IFaceRecognition _faceRecognition;
    private readonly IScannedRoomRepository _scannedRoomRepository;

    private readonly IRoomRepository _roomRepository;
    private readonly ILog _log;

    public ScanRoomService(IFaceRecognition faceRecognition, IScannedRoomRepository scannedRoomRepository, IRoomRepository roomRepository, ILog log)
    {
        _faceRecognition = faceRecognition;
        _scannedRoomRepository = scannedRoomRepository;
        _roomRepository = roomRepository;
        _log = log;
    }

    public async Task<Result> Execute(byte[] imageFile, string roomName)
    {
        var roomExists = await _roomRepository.GetByRoomNameAsync(roomName);
        if(roomExists is null)
        {
            _log.Warn($"Room not found: {roomName}");
            return Result.Failure("Room not found.", ErrorTypes.NotFound);
        }

        _log.Info($"Starting room scan for room: {roomName}");
        var result = await _faceRecognition.VerifyRoomAsync(imageFile);

        if(result.IsFailure)
        {
            _log.Error($"Room scan failed: {result.Error}");
            return Result.Failure(result.Error!, result.ErrorType!.Value);
        }


        var roomScanResult = result.Value;
        var scannedRoom = new ScannedRoom
        {
            Id = Guid.NewGuid().ToString(),
            RoomName = roomName,
            IdentifiedFaces = roomScanResult!.IdentifiedFaces,
            TotalFaces = roomScanResult.TotalFaces,
            ScannedAt = DateTime.UtcNow
        };

        _log.Info($"Scan successful for room: {roomName}. Total Faces: {roomScanResult.TotalFaces}");
        await _scannedRoomRepository.AddScannedRoomResult(scannedRoom);

        return Result.Success();
    }
}

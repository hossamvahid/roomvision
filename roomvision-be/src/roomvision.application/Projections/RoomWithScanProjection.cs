using System;
using roomvision.application.Models;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Projections;

public sealed class RoomWithScanProjection : IProjection<Room, RoomWithScan>
{
    private readonly IScannedRoomRepository _scannedRepository;

    public RoomWithScanProjection(IScannedRoomRepository scannedRepository)
    {
        _scannedRepository = scannedRepository;
    }

    public async Task<IReadOnlyList<RoomWithScan>> ProjectAsync(IReadOnlyList<Room> rooms)
    {
        var result = new List<RoomWithScan>(rooms.Count);

        foreach (var room in rooms)
        {
            var scan = await _scannedRepository.GetScannedRoomResult(room.RoomName!);

            result.Add(new RoomWithScan
            {
                RoomName = room.RoomName!,
                TotalFaces = scan?.TotalFaces ?? 0,
                ScannedAt = scan?.ScannedAt ?? DateTime.MinValue
            });
        }

        return result;
    }
}

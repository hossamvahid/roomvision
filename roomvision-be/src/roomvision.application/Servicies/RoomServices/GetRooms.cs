using System;
using log4net;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.application.Models;
using roomvision.application.Projections;
using roomvision.domain.Common;
using roomvision.domain.Interfaces.Repositories;

namespace roomvision.application.Servicies.RoomServices;

public class GetRooms : IGetRooms
{
    private readonly IRoomRepository _roomRepository;
    private readonly RoomWithScanProjection _projection;
    private readonly ILog _log;

    public GetRooms(IRoomRepository roomRepository, RoomWithScanProjection projection, ILog log)
    {
        _roomRepository = roomRepository;
        _projection = projection;
        _log = log;
    }

    public async Task<Result<Paginated<RoomWithScan>>> Execute(int page, int pageSize = 6)
    {
        _log.Info($"Getting rooms from page: {page} with the size: {pageSize}");
        var rooms = await _roomRepository.GetPagedAsync(page, pageSize);
        _log.Info("Getting the total ammount of rooms");
        var totalItems = await _roomRepository.CountAsync();


        var items = await _projection.ProjectAsync(rooms);
        var paginatedResult = Paginated<RoomWithScan>.Create(items, totalItems, pageSize);

        _log.Info("Rooms successfully retrieved.");
        return Result<Paginated<RoomWithScan>>.Success(paginatedResult);
    }
}

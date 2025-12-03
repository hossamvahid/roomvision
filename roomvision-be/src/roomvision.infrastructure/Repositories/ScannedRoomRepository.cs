using MongoDB.Driver;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Repositories;

public class ScannedRoomRepository : IScannedRoomRepository
{   
    private readonly MongoDbContext _context;
    private readonly IGenericMapper _mapper;

    public ScannedRoomRepository(MongoDbContext context, IGenericMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ScannedRoom?> GetScannedRoomResult(string roomName)
    {
        var recentScan = await _context.ScannedRooms
        .Find(s => s.RoomName == roomName)
        .SortByDescending(s => s.ScannedAt)
        .FirstOrDefaultAsync();

        return _mapper.Map<ScannedRoomDbModel, ScannedRoom>(recentScan);
    }

    public async Task AddScannedRoomResult(ScannedRoom scannedRoom)
    {
        var scannedRoomDbModel = _mapper.Map<ScannedRoom, ScannedRoomDbModel>(scannedRoom);
        await _context.ScannedRooms.InsertOneAsync(scannedRoomDbModel);
    }
}

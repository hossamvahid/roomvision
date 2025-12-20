using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IGenericMapper _mapper;
        private readonly PgSqlContext _context;

        public RoomRepository(IGenericMapper mapper, PgSqlContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room is null)
            {
                return null;
            }
            return _mapper.Map<RoomDbModel, Room>(room);
        }

        public async Task<Room?> GetByRoomNameAsync(string roomName)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomName == roomName);
            if (room is null)
            {
                return null;
            }

            return _mapper.Map<RoomDbModel, Room>(room);
        }

        public async Task<IReadOnlyList<Room>> GetPagedAsync(int page, int pageSize)
        {
            var pagedRooms = await _context.Rooms
            .AsNoTracking()
            .OrderBy(r => r.RoomName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return _mapper.Map<List<RoomDbModel>, List<Room>>(pagedRooms);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Rooms.CountAsync();
        }

        public async Task AddAsync(Room room)
        {
            var mappedRoom = _mapper.Map<Room, RoomDbModel>(room);
            await _context.Rooms.AddAsync(mappedRoom);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            var mappedRoom = _mapper.Map<Room, RoomDbModel>(room);
            _context.Rooms.Update(mappedRoom);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Room room)
        {
            var mappedRoom = _mapper.Map<Room, RoomDbModel>(room);
            _context.Rooms.Remove(mappedRoom);
            await _context.SaveChangesAsync();
        }
    }
}
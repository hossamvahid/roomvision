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
        private readonly ILog _log;

        public RoomRepository(IGenericMapper mapper, PgSqlContext context, ILog log)
        {
            _mapper = mapper;
            _context = context;
            _log = log;
        }

        public async Task<Room?> GetById(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room is null)
            {
                _log.Info($"Room with id {id} not found.");
                return null;
            }

            _log.Info($"Room with id {id} retrieved successfully.");
            return _mapper.Map<RoomDbModel, Room>(room);
        }

        public async Task<Room?> GetByRoomName(string roomName)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomName == roomName);
            if (room is null)
            {
                _log.Info($"Room with name {roomName} not found.");
                return null;
            }

            _log.Info($"Room with name {roomName} retrieved successfully.");
            return _mapper.Map<RoomDbModel, Room>(room);
        }

        public async Task<bool> AddAsync(Room room)
        {
            var mappedRoom = _mapper.Map<Room, RoomDbModel>(room);

            try
            {
                await _context.Rooms.AddAsync(mappedRoom);
                await _context.SaveChangesAsync();
                _log.Info($"Room with name {room.RoomName} added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Room room)
        {
            var mappedRoom = _mapper.Map<Room, RoomDbModel>(room);

            try
            {
                _log.Info($"Updating room with id {room.Id}.");
                _context.Rooms.Update(mappedRoom);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
           
            if (room == null)
            {
                _log.Info($"Room with id {id} not found.");
                return false;
            }

            try
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                _log.Info($"Room with id {id} deleted successfully.");
                return true;
            }
            catch(Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }
    }
}
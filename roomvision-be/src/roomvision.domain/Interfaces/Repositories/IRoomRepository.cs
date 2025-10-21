using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories
{
    public interface IRoomRepository
    {
        public Task<Room?> GetById(int id);
        public Task<Room?> GetByRoomName(string roomName);
        public Task<bool> AddAsync(Room room);
        public Task<bool> UpdateAsync(Room room);
        public Task<bool> DeleteAsync(int id);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Entities;

namespace roomvision.domain.Interfaces.Repositories
{
    public interface IRoomRepository
    {
        public Task<Room?> GetByIdAsync(int id);
        public Task<Room?> GetByRoomNameAsync(string roomName);
        public Task AddAsync(Room room);
        public Task UpdateAsync(Room room);
        public Task DeleteAsync(Room room);

    }
}
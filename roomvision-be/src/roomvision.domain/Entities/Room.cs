using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public string? Password { get; set; }
    }
}
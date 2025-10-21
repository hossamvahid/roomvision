using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.infrastructure.Models
{
    public class RoomDbModel
    {
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public string? Password { get; set; }
    }
}
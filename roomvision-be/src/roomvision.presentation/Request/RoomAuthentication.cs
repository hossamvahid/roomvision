using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.presentation.Request
{
    public class RoomAuthentication
    {
        [Required]
        public string? RoomName { get; set; }
        
        [Required]
        public string? Password { get; set; }
    }
}
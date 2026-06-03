using System.ComponentModel.DataAnnotations;

namespace roomvision.presentation.Request.RoomRequest
{
    public class CreateRoom
    {
        [Required]
        public string? RoomName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}

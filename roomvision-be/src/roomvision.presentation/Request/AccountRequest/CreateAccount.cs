using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using roomvision.domain.Enums;

namespace roomvision.presentation.Request.AccountRequest
{
    public class CreateAccount
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
    }
}
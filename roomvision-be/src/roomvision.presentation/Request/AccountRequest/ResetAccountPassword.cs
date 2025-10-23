using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.presentation.Request.AccountRequest
{
    public class ResetAccountPassword
    {
        [Required]
        public string? NewPassword { get; set; }
    }
}
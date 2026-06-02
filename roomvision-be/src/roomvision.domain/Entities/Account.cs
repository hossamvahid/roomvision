using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roomvision.domain.Enums;

namespace roomvision.domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Role Role { get; set; }
    }
}
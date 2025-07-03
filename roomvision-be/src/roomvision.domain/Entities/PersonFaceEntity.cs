using roomvision.domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.domain.Entities
{
    public class PersonFaceEntity
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Encoding { get; set; }
        public Role role { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.UserDTO
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ProfilePictureUrl { get; set; }
    }
}

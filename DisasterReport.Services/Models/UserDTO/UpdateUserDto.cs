using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.UserDTO
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}

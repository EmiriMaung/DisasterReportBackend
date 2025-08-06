using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.UserDTO
{
    public class UpdateUserFormDto
    {
        public string Name { get; set; } = string.Empty;

        public IFormFile? ProfilePicture { get; set; }
    }
}

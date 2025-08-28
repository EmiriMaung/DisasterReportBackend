using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.UserDTO
{
    public class UpdateUserNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}

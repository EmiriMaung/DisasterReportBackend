using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.AuthDTO
{
    public class OtpRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}

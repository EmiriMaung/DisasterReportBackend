using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}

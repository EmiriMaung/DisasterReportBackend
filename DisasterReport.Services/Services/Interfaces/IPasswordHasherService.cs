using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
    }
}

using CP.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Status> RegisterAsync(RegistrationModel model);
        Task<Status> LoginAsync(LoginModel model);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Interfaces
{
    public interface IUserService
    {
        Task<Guid> ObtenerUsuarioPorEmailAsync(string correo);
    }
}

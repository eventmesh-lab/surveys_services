using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.application.Interfaces;

namespace surveys_services.infrastructure.Services
{
    public class UserService : IUserService
    {

        /// <summary>
        /// Atributo que se encarga de procesar las solicitudes a servicios externos.
        /// </summary>
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Método que se encarga de obtener el ID de un usuario por su correo en el Microservicio Usuarios.
        /// </summary>
        /// <param name="correo">Parametro que corresponde al correo del usuario a consultar</param>
        /// <returns>Retorna un valor GUID que corresponde al ID del usuario consultado.
        /// Si no lo consigue, retorna un GUID vacio</returns>
        public async Task<Guid> ObtenerUsuarioPorEmailAsync(string correo)
        {
            var response = await _httpClient.GetAsync($"http://localhost:7181/api/users/getIdUser/{correo}");

            if (!response.IsSuccessStatusCode)
            {
                return Guid.Empty;
            }

            var guidString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GUID recibido desde el microservicio (antes de conversión): {guidString}");

            if (Guid.TryParse(guidString.Trim('"'), out Guid userId))
            {
                return userId;
            }
            else
            {
                return Guid.Empty;
            }
        }

    }
}

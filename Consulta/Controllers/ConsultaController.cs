using Microsoft.AspNetCore.Mvc;
using Consulta.Services;
using Consulta.Models;
using System.Globalization;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Consulta.Controllers
{
    [Route("Consulta")]
    [ApiController]
    public class ConsultaController(IConsultaService consultaService) : ControllerBase
    {

        #region | Construtores

        private readonly IConsultaService _consultaService = consultaService;

        private static readonly string chavePrivada = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC5Nc/9hiCV07lrKhqWDeTg7ml+z5gpUYGfdb8rtTYCv5EcX92/L55rb6NZFr2dFI9EwA0QNxk6k/qo56mQla8tX89udn201gSIvgIO8z0FFECa1Yq15QHHR92zRzumzHV8pByO4uNjAAXlR+dXLfe0sN9Vj8QH2iHEyJX8nU6VHiBJWp+JaGSYM/aNZUQ3bL9w2KU5ZYtI8+l0Pcib6dPJsXowJHHuqV+G3PYCQo47hOeHVD7vRb8oXIP2B5+gQhe3v5q0d9t1Tteg01seudMcZqC5U42xa9F2h4UlDIj1lHIBkzQ8v4N7vHBEyEKDU/tNa9cmQUlZ5RbqrjQa6nrlAgMBAAECggEBAJX7FM4Z4qNRy/ITK2peH/1iM33kgDTdcxl9UW0ElpNNLCaNH1XmKuSXh/Dp8GyWrJog57M82ypLkQ1cZDzYaf5VevXZ8FwTf6J4M9SFduF5EAQSbvxzUahd8xNt2MlvAqkHgLTY2QhfiWatfpJjgBcLsB9qh3VQWE4xnPvvKyam13hB2iwNWXsBqtY8I8j2Eh36xNlFHjR1T04y5gd/ZQIe5owwGwDplk0pDLOzynABSC7gA+8ZmWOOEjzk5GrFCLJeEvVpiwNtEVPOVTKP0sSaMjrySUXrKeFEO7WZbcp7Ju68/CeABOidKVLwAu6h/Fj7eb/Vrp1NyXpTNEuJjdUCgYEA76K6ew8REXS/K/dNXOCTD0H01ZNkzZbxu3XXtFpNuVVGE8fp3DRHuwIXqOijHHe/+N6DKDxDTNxnthWp5z7strxYqcOqIBpl13zEVxe/qwcx9P1gBqCAFXMYh7MbvSubldufCWjpArj1K4QT3z6ih9prm7aftP0FiXxwd3OmNIMCgYEAxduginE6AOqWrcUTmwq5FZKlJZylIGGVuWCw2v3uTBVm9ruM/hRjqaqxcodRGzzZ2hNqUy23v6slPYChA7k/otQr/mCP6K8PRyu8HrZuNEZxEwiaReuxrBVZLCO6YLFo86PtASwaA42bKmBKi9K38tlMbJpRrpbml2Gl35VPBncCgYEArKFiDbyw4w5jmdyyErKFvnwZULK575FJ223tUrOrQlQ4A5AE3Omcsw+y06+jsaJi3XoOqjGfmgM9g2Lf3wLprErK5KFcMXxS2YW8O7GUFjU8u+y7/IorO0iK51cUKJb9oltwmmrFUXzEwfCIEE8i7xNeafKJ4uxTzNOkgakCltkCgYBx1Rb7L4NojY7dMNpDxBqSD8mV5xaVl681dSyrAZc9DL8tSuAmOgLQ1ZS7yKBgJFVOAweUDWR/EG8fgEaixsyW2Kzq8BE7lKQclUvo+5pR1wktIzDm82BIGgwwuel7wfYY3oH3v7DDxI+2BKo+4Z/VRzGR9gLyiHzoMVTmWpeZpwKBgG3kXaEW0s/vVhIiOLduKxL5LVtsrzHK6+Qrm8nGIQ18gT9ply4ZD4hyjQxYCQwjFtLsJfJKF+lBBqNza0FFL/n6eeHWQdOuccEE+4RpgsDkzTzlFUrxHsC3hWjVxSTgxZxu4+8eVSdm7H6mjyFxRv9mS9xnwO5sGJRphqrDXXCE";

        #endregion

        #region | Classes

        public class Login
        {
            public string login { get; set; }
            public string senha { get; set; }
        }

        #endregion

        #region | Endpoints

        [HttpGet("Medicos")]
        public async Task<ActionResult<IEnumerable<Medico>>> Get_MedicosDisponiveis()
        {
            var queryParams = HttpContext.Request.Query;

            string? id = queryParams.ContainsKey("id") ? queryParams["id"].ToString() : null;
            string? data = queryParams.ContainsKey("data") ? queryParams["data"].ToString() : null;
            string? especialidade = queryParams.ContainsKey("especialidade") ? queryParams["especialidade"].ToString() : null;

            if (!DateTime.TryParseExact(HttpUtility.UrlDecode(data), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataConvertida))
            {
                if (!string.IsNullOrEmpty(data))
                    return NotFound();
            }

            var medicos = await _consultaService.Get_MedicosDisponiveis(id, !string.IsNullOrEmpty(data) ? dataConvertida : null, especialidade);

            if (medicos == null || !medicos.Any())
                return NotFound();

            return Ok(medicos);
        }

        [HttpGet("Consultas")]
        public async Task<IActionResult> Get_Consultas()
        {
            try
            {
                var queryParams = HttpContext.Request.Query;
                string id = queryParams["id"].ToString();
                int.TryParse(queryParams["idTipo"].ToString(), out int idTipo);

                return Ok(await _consultaService.Get_Consultas(id, idTipo));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("Medicos/Valor")]
        public async Task<IActionResult> Get_ValorConsulta_Medico([FromQuery] string id)
        {
            try
            {
                return Ok(await _consultaService.Get_ValorConsulta_Medico(id));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost("Medicos/Login")]
        public async Task<IActionResult> GetLogin_Medico([FromBody] Login login)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                byte[] privateKeyBytes = Convert.FromBase64String(chavePrivada);
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                string senha = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(login.senha), false));

                var medicoLogado = await _consultaService.Get_LoginMedico(login.login, senha);

                if (medicoLogado == null)
                    return NotFound();

                return Ok(medicoLogado);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost("Pacientes/Login")]
        public async Task<IActionResult> GetLogin_Paciente([FromBody] Login login)
        {
            var rsa = new RSACryptoServiceProvider();
            byte[] privateKeyBytes = Convert.FromBase64String(chavePrivada);
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

            string senha = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(login.senha), false));

            try
            {
                var pacienteLogado = await _consultaService.Get_LoginPaciente(login.login, senha);

                if (pacienteLogado == null)
                    return NotFound();

                return Ok(pacienteLogado);
            }
            catch (Exception ex)
            {
                return NotFound($"Não foi possível encontrar o Paciente!\r\nErro: {ex.Message}.");
            }
        }

        #endregion
    }
}

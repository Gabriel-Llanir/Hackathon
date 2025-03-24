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

        private static readonly string chavePrivada = Environment.GetEnvironmentVariable("RSA_PRIVATE_KEY");

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

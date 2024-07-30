using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContatosAPI.Models;
using ContatosAPI.Services;
using Prometheus;

namespace ContatosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds", new HistogramConfiguration { LabelNames = ["method", "endpoint"] });
        private static readonly Counter RequestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests made", new CounterConfiguration { LabelNames = ["method", "endpoint", "status"] });

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            using (RequestDuration.WithLabels("GET", "api/Usuarios").NewTimer())
            {
                var usuarios = await _usuarioService.GetAllAsync();

                var response = Ok(usuarios);
                RequestCounter.WithLabels("GET", "api/Usuarios", response.StatusCode.ToString()).Inc();

                return response;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            using (RequestDuration.WithLabels("GET", "api/Usuarios/{id}").NewTimer())
            {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                {
                    RequestCounter.WithLabels("GET", "api/Usuarios/{id}", NotFound().StatusCode.ToString()).Inc();
                 
                    return NotFound();
                }

                RequestCounter.WithLabels("GET", "api/Usuarios/{id}", Ok(usuario).StatusCode.ToString()).Inc();

                return Ok(usuario);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetUsuariosByDDD([FromQuery] string DDD)
        {
            var usuarios = await _usuarioService.GetAllAsync(DDD);
            if (!(usuarios.Count() > 1)) return NotFound();
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            var usuarios = await _usuarioService.GetAllAsync();
            usuario.IdUsuario = usuarios.Last().IdUsuario + 1;

            await _usuarioService.AddAsync(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            var existingUsuario = await _usuarioService.GetByIdAsync(id);
            if (existingUsuario == null) return NotFound();
            usuario.Id = existingUsuario.Id;

            await _usuarioService.UpdateAsync(id, usuario);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (RequestDuration.WithLabels("DELETE", "api/Usuarios/{id}").NewTimer())
            {
                var existingUsuario = await _usuarioService.GetByIdAsync(id);

                if (existingUsuario == null)
                {
                    RequestCounter.WithLabels("DELETE", "api/Usuarios/{id}", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }

                RequestCounter.WithLabels("DELETE", "api/Usuarios/{id}", NoContent().StatusCode.ToString()).Inc();

                await _usuarioService.RemoveAsync(id);
                return NoContent();
            }
        }

    }
}

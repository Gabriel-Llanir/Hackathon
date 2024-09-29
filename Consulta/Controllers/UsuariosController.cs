using Microsoft.AspNetCore.Mvc;
using Consulta.Services;
using Consulta.Models;

namespace Consulta.Controllers
{
    [Route("Consulta")]
    [ApiController]
    public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        #region | Endpoints

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
                var usuarios = await _usuarioService.GetAllAsync();
                return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                    return NotFound();

                return Ok(usuario);
        }

        [HttpGet("DDD/{DDD}")]
        public async Task<IActionResult> GetUsuariosByDDD(string DDD)
        {
            var usuarios = await _usuarioService.GetAllAsync(DDD);
            if (!(usuarios.Count() > 1)) return NotFound();
            return Ok(usuarios);
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContatosAPI.Models;
using ContatosAPI.Services;

namespace ContatosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

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
            if (usuario == null) return NotFound();
            return Ok(usuario);
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
            var existingUsuario = await _usuarioService.GetByIdAsync(id);
            if (existingUsuario == null) return NotFound();

            await _usuarioService.RemoveAsync(id);
            return NoContent();
        }

    }
}

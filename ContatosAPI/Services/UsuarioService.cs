using System.Collections.Generic;
using System.Threading.Tasks;
using ContatosAPI.Models;
using ContatosAPI.Repositories;

namespace ContatosAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(string DDD)
        {
            return await _usuarioRepository.GetAllAsync(DDD);
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _usuarioRepository.AddAsync(usuario);
        }

        public async Task UpdateAsync(int id, Usuario usuarioIn)
        {
            await _usuarioRepository.UpdateAsync(id, usuarioIn);
        }

        public async Task RemoveAsync(int id)
        {
            await _usuarioRepository.RemoveAsync(id);
        }
    }
}

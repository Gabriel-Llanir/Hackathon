using System.Collections.Generic;
using System.Threading.Tasks;
using ContatosAPI.Models;

namespace ContatosAPI.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllAsync(string DDD);
        Task<Usuario> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(int id, Usuario usuarioIn);
        Task RemoveAsync(int id);
    }
}

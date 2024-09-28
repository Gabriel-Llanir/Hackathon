using Consulta.Models;

namespace Consulta.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllAsync(string DDD);
        Task<Usuario> GetByIdAsync(int id);
    }
}

using Consulta.Models;

namespace Consulta.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllAsync(string DDD);
        Task<Usuario> GetByIdAsync(int id);
    }
}

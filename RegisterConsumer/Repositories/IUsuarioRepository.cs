using RegisterConsumer.Models;

namespace RegisterConsumer.Repositories
{
    public interface IUsuarioRepository
    {
        Task AddAsync(Usuario usuario);
    }
}

using RegisterConsumer.Models;

namespace RegisterConsumer.Services
{
    public interface IUsuarioService
    {
        Task AddAsync(Usuario? usuario);
    }
}

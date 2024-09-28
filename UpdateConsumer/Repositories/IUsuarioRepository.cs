using UpdateConsumer.Models;

namespace UpdateConsumer.Repositories
{
    public interface IUsuarioRepository
    {
        Task UpdateAsync(int id, Usuario usuarioIn);
    }
}

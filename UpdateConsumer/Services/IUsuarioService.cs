using UpdateConsumer.Models;

namespace UpdateConsumer.Services
{
    public interface IUsuarioService
    {
        Task UpdateAsync(int? id, Usuario? usuarioIn);
    }
}

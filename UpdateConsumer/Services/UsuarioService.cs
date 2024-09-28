using UpdateConsumer.Models;
using UpdateConsumer.Repositories;

namespace UpdateConsumer.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

        public async Task UpdateAsync(int? id, Usuario? usuario)
        {
            int idUsuario = (int)(id == null ? 0 : id);

            if (usuario != null && id != null)
                await _usuarioRepository.UpdateAsync(idUsuario, usuario);
            else
                throw new Exception("É necessário enviar um Usuário para atualizar suas informações!");
        }
    }
}

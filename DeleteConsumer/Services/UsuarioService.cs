using DeleteConsumer.Repositories;

namespace DeleteConsumer.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

        public async Task RemoveAsync(int id)
        => await _usuarioRepository.RemoveAsync(id);
    }
}

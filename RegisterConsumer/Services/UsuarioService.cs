using RegisterConsumer.Models;
using RegisterConsumer.Repositories;

namespace RegisterConsumer.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

        public async Task AddAsync(Usuario? usuario)
        {
            if (usuario != null)
                await _usuarioRepository.AddAsync(usuario);
            else
                throw new Exception("É necessário passar um Usário para ser Cadastrado!");
        }
    }
}

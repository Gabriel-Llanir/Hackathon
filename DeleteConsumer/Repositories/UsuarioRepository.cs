using DeleteConsumer.Data;
using MongoDB.Driver;

namespace DeleteConsumer.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        => _context = context;

        public async Task RemoveAsync(int id)
        => await _context.Usuarios.DeleteOneAsync(u => u.IdUsuario == id);
    }
}

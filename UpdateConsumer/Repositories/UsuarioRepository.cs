using UpdateConsumer.Data;
using UpdateConsumer.Models;
using MongoDB.Driver;

namespace UpdateConsumer.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        => _context = context;

        public async Task UpdateAsync(int id, Usuario usuario)
        => await _context.Usuarios.ReplaceOneAsync(u => u.IdUsuario == id, usuario);
    }
}

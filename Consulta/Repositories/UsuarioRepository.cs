using Consulta.Data;
using Consulta.Models;
using MongoDB.Driver;

namespace Consulta.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(string DDD)
        {
            return await _context.Usuarios.Find(usuario => usuario.CodigoRegiao == DDD.ToUpper()).ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios.Find(u => u.IdUsuario == id).FirstOrDefaultAsync();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using ContatosAPI.Data;
using ContatosAPI.Models;
using MongoDB.Driver;

namespace ContatosAPI.Repositories
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

        public async Task AddAsync(Usuario usuario)
        {
            var regiao = await _context.Regioes.Find(r => r.DDDRegiao == usuario.Telefone.Substring(0, 2)).FirstOrDefaultAsync();
            if (regiao != null)
            {
                usuario.CodigoRegiao = regiao.SiglaRegiao;
                await _context.Usuarios.InsertOneAsync(usuario);
            }
            else
            {
                throw new Exception("Região não encontrada para o DDD fornecido.");
            }
        }

        public async Task UpdateAsync(int id, Usuario usuarioIn)
        {
            await _context.Usuarios.ReplaceOneAsync(u => u.IdUsuario == id, usuarioIn);
        }

        public async Task RemoveAsync(int id)
        {
            await _context.Usuarios.DeleteOneAsync(u => u.IdUsuario == id);
        }
    }
}

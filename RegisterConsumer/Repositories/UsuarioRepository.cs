using RegisterConsumer.Data;
using RegisterConsumer.Models;
using MongoDB.Driver;

namespace RegisterConsumer.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        => _context = context;

        public async Task AddAsync(Usuario usuario)
        {
            var regiao = await _context.Regioes.Find(r => r.DDDRegiao == usuario.Telefone.Substring(0, 2)).FirstOrDefaultAsync();

            if (regiao != null)
            {
                usuario.CodigoRegiao = regiao.SiglaRegiao;

                if (usuario.IdUsuario <= 0)
                {
                    var proxID = await _context.Usuarios.Find(_ => true).ToListAsync();

                    if (proxID != null)
                        usuario.IdUsuario = proxID.Last().IdUsuario + 1;
                }

                usuario.Atualizacao = DateTime.Now;

                await _context.Usuarios.InsertOneAsync(usuario);
            }
            else
                throw new Exception("Região não encontrada para o DDD fornecido!");
        }
    }
}

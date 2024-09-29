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
        {
            if (id > 0)
            {
                var regiao = await _context.Regioes.Find(r => r.DDDRegiao == usuario.Telefone.Substring(0, 2)).FirstOrDefaultAsync();

                if (regiao != null)
                {
                    usuario.CodigoRegiao = regiao.SiglaRegiao;

                    var usuarioOriginal = await _context.Usuarios.Find(u => u.IdUsuario.Equals(id)).FirstOrDefaultAsync();

                    if (usuarioOriginal != null)
                    {
                        usuario.Id = usuarioOriginal.Id;
                        usuario.IdUsuario = usuarioOriginal.IdUsuario;
                        usuario.Atualizacao = DateTime.Now;

                        await _context.Usuarios.ReplaceOneAsync(u => u.IdUsuario == id, usuario);
                    }
                    else
                        throw new Exception("Não foi possível encontrar um Usuário, para ser Atualizado, a partir do ID fornecido!");
                }
                else
                    throw new Exception("Região não encontrada para o DDD fornecido!");
            }
            else
                throw new Exception("É necessário passar o ID, do Usuário a ser atualizado, na URL da Requisição!");
        }
    }
}

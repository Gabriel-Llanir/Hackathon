using Xunit;
using Moq;
using Consulta.Services;
using Consulta.Repositories;
using Consulta.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task GetAllUsuarios_ReturnsAllUsuarios()
        {
            var mockRepository = new Mock<IUsuarioRepository>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestUsuarios());
            var service = new UsuarioService(mockRepository.Object);

            var usuarios = await service.GetAllAsync();

            Assert.Equal(2, usuarios.Count());
        }

        private IEnumerable<Usuario> GetTestUsuarios()
        {
            return new List<Usuario>
            {
                new Usuario { IdUsuario = 1, Nome = "Teste1", Telefone = "11999999999", Email = "teste1@example.com" },
                new Usuario { IdUsuario = 2, Nome = "Teste2", Telefone = "21999999999", Email = "teste2@example.com" }
            };
        }
    }
}

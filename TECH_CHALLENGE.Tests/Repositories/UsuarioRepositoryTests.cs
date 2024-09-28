using Xunit;
using Moq;
using Consulta.Repositories;
using Consulta.Data;
using Consulta.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace TECH_CHALLENGE.Tests.Repositories
{
    public class UsuarioRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllUsuarios()
        {

            var mockContext = new Mock<DataContext>();
            var mockCollection = new Mock<IMongoCollection<Usuario>>();
            var mockCursor = new Mock<IAsyncCursor<Usuario>>();

            var testUsuarios = GetTestUsuarios();

            mockCursor.Setup(_ => _.Current).Returns(testUsuarios);
            mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                      .Returns(true)
                      .Returns(false);
            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);

            mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Usuario>>(), It.IsAny<FindOptions<Usuario, Usuario>>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockCursor.Object);

            mockContext.Setup(c => c.Usuarios).Returns(mockCollection.Object);

            var repository = new UsuarioRepository(mockContext.Object);

            var usuarios = await repository.GetAllAsync();

            Assert.Equal(2, usuarios.Count());
        }

        private List<Usuario> GetTestUsuarios()
        {
            return new List<Usuario>
            {
                new Usuario { IdUsuario = 1, Nome = "Teste1", Telefone = "11999999999", Email = "teste1@example.com" },
                new Usuario { IdUsuario = 2, Nome = "Teste2", Telefone = "21999999999", Email = "teste2@example.com" }
            };
        }
    }
}

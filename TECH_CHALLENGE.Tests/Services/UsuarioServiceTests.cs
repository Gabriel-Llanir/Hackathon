using Moq;
using Consulta.Models;
using Consulta.Services;
using Consulta.Repositories;
using RegisterRepository = RegisterConsumer.Repositories.IUsuarioRepository;
using RegisterUsuario = RegisterConsumer.Models.Usuario;
using RegisterUsuarioService = RegisterConsumer.Services.UsuarioService;
using UpdateRepository = UpdateConsumer.Repositories.IUsuarioRepository;
using UpdateUsuario = UpdateConsumer.Models.Usuario;
using UpdateUsuarioService = UpdateConsumer.Services.UsuarioService;
using DeleteRepository = DeleteConsumer.Repositories.IUsuarioRepository;
using DeleteUsuarioService = DeleteConsumer.Services.UsuarioService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TECH_CHALLENGE.Services
{
    public class UsuarioServiceTests
    {
        [Fact]
        public async Task AddUsuario_AddsUsuario()
        {
            var mockRepository = new Mock<RegisterRepository>();
            var usuario = new RegisterUsuario { IdUsuario = 1, Nome = "Teste1", Telefone = "11999999999", Email = "teste1@example.com", CodigoRegiao = "SP" };
            mockRepository.Setup(repo => repo.AddAsync(usuario)).Returns(Task.CompletedTask);
            var service = new RegisterUsuarioService(mockRepository.Object);

            await service.AddAsync(usuario);

            mockRepository.Verify(repo => repo.AddAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task GetAllUsuarios_ReturnsAllUsuarios()
        {
            var mockRepository = new Mock<IUsuarioRepository>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestUsuarios());
            var service = new UsuarioService(mockRepository.Object);

            var usuarios = await service.GetAllAsync();

            Assert.Equal(2, usuarios.Count());
        }

        [Fact]
        public async Task UpdateUsuario_UpdatesUsuario()
        {

            var mockRepository = new Mock<UpdateRepository>();
            var usuario = new UpdateUsuario { IdUsuario = 1, Nome = "Teste Atualizado", Telefone = "11999999999", Email = "teste.atualizado@example.com", CodigoRegiao = "SP" };
            mockRepository.Setup(repo => repo.UpdateAsync(usuario.IdUsuario, usuario)).Returns(Task.CompletedTask);
            var service = new UpdateUsuarioService(mockRepository.Object);

            await service.UpdateAsync(usuario.IdUsuario, usuario);

            mockRepository.Verify(repo => repo.UpdateAsync(usuario.IdUsuario, usuario), Times.Once);
        }

        [Fact]
        public async Task RemoveUsuario_RemovesUsuario()
        {
            var mockRepository = new Mock<DeleteRepository>();
            var usuarioId = 1;
            mockRepository.Setup(repo => repo.RemoveAsync(usuarioId)).Returns(Task.CompletedTask);
            var service = new DeleteUsuarioService(mockRepository.Object);

            await service.RemoveAsync(usuarioId);

            mockRepository.Verify(repo => repo.RemoveAsync(usuarioId), Times.Once);
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

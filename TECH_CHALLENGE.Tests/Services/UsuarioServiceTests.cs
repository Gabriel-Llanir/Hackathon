using Xunit;
using Moq;
using ContatosAPI.Repositories;
using ContatosAPI.Services;
using ContatosAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TECH_CHALLENGE.Tests.Services
{
    public class UsuarioServiceTests
    {
        [Fact]
        public async Task AddUsuario_AddsUsuario()
        {
            var mockRepository = new Mock<IUsuarioRepository>();
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste1", Telefone = "11999999999", Email = "teste1@example.com" };
            mockRepository.Setup(repo => repo.AddAsync(usuario)).Returns(Task.CompletedTask);
            var service = new UsuarioService(mockRepository.Object);

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
 
            var mockRepository = new Mock<IUsuarioRepository>();
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste Atualizado", Telefone = "11999999999", Email = "teste.atualizado@example.com" };
            mockRepository.Setup(repo => repo.UpdateAsync(usuario.IdUsuario, usuario)).Returns(Task.CompletedTask);
            var service = new UsuarioService(mockRepository.Object);

            await service.UpdateAsync(usuario.IdUsuario, usuario);

            mockRepository.Verify(repo => repo.UpdateAsync(usuario.IdUsuario, usuario), Times.Once);
        }

        [Fact]
        public async Task RemoveUsuario_RemovesUsuario()
        {
            var mockRepository = new Mock<IUsuarioRepository>();
            var usuarioId = 1;
            mockRepository.Setup(repo => repo.RemoveAsync(usuarioId)).Returns(Task.CompletedTask);
            var service = new UsuarioService(mockRepository.Object);

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

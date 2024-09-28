using Xunit;
using Moq;
using Consulta.Controllers;
using Consulta.Services;
using Consulta.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TECH_CHALLENGE.Tests.Controllers
{
    public class UsuariosControllerTests
    {
        [Fact]
        public async Task GetUsuarios_ReturnsOkResult_WithListOfUsuarios()
        {
            var mockService = new Mock<IUsuarioService>();
            mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(GetTestUsuarios());
            var controller = new UsuariosController(mockService.Object);

            var result = await controller.GetUsuarios();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Usuario>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
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

using Microsoft.AspNetCore.Mvc;
using Prometheus;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Gateway.Models;
using Gateway.Services;

namespace Gateway.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
    {
        #region | Construtores

        private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds", new HistogramConfiguration { LabelNames = ["method", "endpoint"] });
        private static readonly Counter RequestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests made", new CounterConfiguration { LabelNames = ["method", "endpoint", "status"] });

        private readonly IUsuarioService _usuarioService = usuarioService;

        private readonly ConnectionFactory factory = new()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        #endregion

        #region | Endpoints

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            using (RequestDuration.WithLabels("GET", "api").NewTimer())
            {
                var usuarios = await _usuarioService.GetAllAsync();

                var response = Ok(usuarios);
                RequestCounter.WithLabels("GET", "api", response.StatusCode.ToString()).Inc();

                return response;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            using (RequestDuration.WithLabels("GET", "api/{id}").NewTimer())
            {
                var usuario = await _usuarioService.GetByIdAsync(id);

                if (usuario == null)
                {
                    RequestCounter.WithLabels("GET", "api/{id}", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }

                RequestCounter.WithLabels("GET", "api/{id}", Ok(usuario).StatusCode.ToString()).Inc();

                return Ok(usuario);
            }
        }

        [HttpGet("DDD")]
        public async Task<IActionResult> GetUsuariosByDDD([FromQuery] string DDD)
        {
            var usuarios = await _usuarioService.GetAllAsync(DDD);
            if (!(usuarios.Count() > 1)) return NotFound();
            return Ok(usuarios);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            using (var channel = factory.CreateConnection().CreateModel())
            {
                channel.QueueDeclare(
                    queue: "RegisterQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "RegisterQueue",
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(usuario)));
            }

            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Usuario usuario)
        {
            using (var channel = factory.CreateConnection().CreateModel())
            {
                channel.QueueDeclare(
                    queue: "UpdateQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "UpdateQueue",
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Dictionary<int, Usuario> { { id, usuario } })));
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (RequestDuration.WithLabels("DELETE", "api/{id}").NewTimer())
            {
                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "DeleteQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "DeleteQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(id)));
                }

                return NoContent();
            }
        }

        #endregion
    }
}

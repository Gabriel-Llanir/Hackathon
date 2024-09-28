using UpdateConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UpdateConsumer.Services;

namespace UpdateConsumer
{
    public class Worker(IUsuarioService usuarioService) : BackgroundService
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "UpdateQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                bool ack = false;
                consumer.Received += async (sender, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var usuario = JsonSerializer.Deserialize<Dictionary<int, Usuario>>(body);

                        await _usuarioService.UpdateAsync(usuario.First().Key, usuario.First().Value);
                        ack = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        ack = false;
                    }
                };

                channel.BasicConsume(
                    queue: "UpdateQueue",
                    autoAck: ack,
                    consumer: consumer);
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
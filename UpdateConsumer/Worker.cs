using UpdateConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UpdateConsumer.Services;

namespace UpdateConsumer
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();

                _channel.QueueDeclare(queue: "UpdateQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (sender, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var usuario = JsonSerializer.Deserialize<Dictionary<int, Usuario>>(body);

                        await usuarioService.UpdateAsync(usuario.First().Key, usuario.First().Value);
                        _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                _channel.BasicConsume(queue: "UpdateQueue", autoAck: false, consumer: consumer);
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
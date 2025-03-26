using System.Text;
using System.Text.Json;
using RegisterConsumer.Models;
using RegisterConsumer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RegisterConsumer
{
    public class Consultas_Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public Consultas_Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "guest", Password = "guest", RequestedHeartbeat = TimeSpan.FromSeconds(30), AutomaticRecoveryEnabled = true, NetworkRecoveryInterval = TimeSpan.FromSeconds(10) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var consultaService = scope.ServiceProvider.GetRequiredService<IRegisterService>();

                _channel.QueueDeclare(queue: "Consultas_RegisterQueue",
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
                        var consulta = JsonSerializer.Deserialize<Consulta>(body);

                        await consultaService.AddAsync(consulta);
                        _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                _channel.BasicConsume(queue: "Consultas_RegisterQueue", autoAck: false, consumer: consumer);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
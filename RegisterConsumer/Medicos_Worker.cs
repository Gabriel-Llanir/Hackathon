using System.Text;
using System.Text.Json;
using RegisterConsumer.Models;
using RegisterConsumer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RegisterConsumer
{
    public class Medicos_Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public Medicos_Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory() { HostName = "rabbitmq-service", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var medicoService = scope.ServiceProvider.GetRequiredService<IRegisterService>();

                _channel.QueueDeclare(queue: "Medicos_RegisterQueue",
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
                        var medico = JsonSerializer.Deserialize<Medico>(body);

                        await medicoService.AddAsync(medico);
                        _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                _channel.BasicConsume(queue: "Medicos_RegisterQueue", autoAck: false, consumer: consumer);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
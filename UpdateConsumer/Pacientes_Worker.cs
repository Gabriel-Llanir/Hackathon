using System.Text;
using System.Text.Json;
using UpdateConsumer.Models;
using UpdateConsumer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UpdateConsumer
{
    public class Pacientes_Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public Pacientes_Worker(IServiceProvider serviceProvider)
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
                var usuarioService = scope.ServiceProvider.GetRequiredService<IUpdateService>();

                _channel.QueueDeclare(queue: "Pacientes_UpdateQueue",
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
                        var paciente = JsonSerializer.Deserialize<Paciente>(body);

                        await usuarioService.UpdateAsync(paciente);
                        _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                _channel.BasicConsume(queue: "Pacientes_UpdateQueue", autoAck: false, consumer: consumer);
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
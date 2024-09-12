using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMq.Reciever.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMq.Reciever.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public ConsumerService(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = _configuration["MessageBroker:Host"],
                Port = int.Parse(_configuration["MessageBroker:Port"]),
                UserName = _configuration["MessageBroker:Username"],
                Password = _configuration["MessageBroker:Password"],
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _configuration["MessageBroker:Queue"],
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message
                HandleMessage(message);

                // Acknowledge that the message has been processed
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _configuration["MessageBroker:Queue"],
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(string message)
        {
            // Deserialize and handle the message as needed
            var deserializedMessage = JsonConvert.DeserializeObject<Message>(message);
            // Process the message...
            Console.WriteLine($"Received message: {deserializedMessage}");
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }

    }
}

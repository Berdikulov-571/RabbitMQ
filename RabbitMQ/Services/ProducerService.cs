using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Interfaces;
using RabbitMQ.Models.Messages;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RabbitMQ.Services
{
    public class ProducerService : IProducerService
    {
        private readonly IConfiguration _config;
        private readonly IModel _channel;

        public ProducerService(IConfiguration configuration)
        {
            _config = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = _config["MessageBroker:Host"],
                Port = int.Parse(_config["MessageBroker:Port"]),
                UserName = _config["MessageBroker:Username"],
                Password = _config["MessageBroker:Password"],
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: _config["MessageBroker:Queue"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public void Send(Message message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "",
                routingKey: _config["MessageBroker:Queue"],
                basicProperties: null,
                body: body);
        }
    }
}

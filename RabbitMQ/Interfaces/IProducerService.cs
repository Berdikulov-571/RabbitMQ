using RabbitMQ.Models.Messages;

namespace RabbitMQ.Interfaces
{
    public interface IProducerService
    {
        public void Send(Message message);
    }
}
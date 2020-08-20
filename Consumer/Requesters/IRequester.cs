using Common;
using RabbitMQ.Client.Events;

namespace Consumer.Requesters
{
    public interface IRequester
    {
        bool TryRequest(Message message);
    }
}
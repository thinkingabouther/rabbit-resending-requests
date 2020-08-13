using RabbitMQ.Client.Events;

namespace Consumer
{
    public interface IRequester
    {
        bool TryRequest(BasicDeliverEventArgs eventArgs);
    }
}
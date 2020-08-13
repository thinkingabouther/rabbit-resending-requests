using System;
using System.Text;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Producer
{
    public class RabbitMessageProducer : IDisposable
    {
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        
        private IModel _channel = new ConnectionFactory().CreateConnection().CreateModel();

        public RabbitMessageProducer(string exchangeName, string routingKey)
        {
            RoutingKey = routingKey;
            ExchangeName = exchangeName;
        }

        private void InitializeExchange()
        {
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        }

        public void Publish(string message)
        {
            InitializeExchange();
            var messageModel = GetMessage(message);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel));
            _channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKey, basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
        
        private static Message GetMessage(string url)
        {
            var message = new Message {Url = url};
            return message;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Consumer.FailurePostProcessors
{
    public class RabbitMessageRePublisher : IFailurePostProcessor, IDisposable
    {
        public string ExchangeName { get; }
        public string QueueName { get; }
        public string DeadLetterExchangeName { get; }
        public string RoutingKeys { get; }
        public int Delay = 10000;
        
        private readonly IModel _channel = new ConnectionFactory().CreateConnection().CreateModel();

        public RabbitMessageRePublisher(string exchangeName, string queueName, string deadLetterExchangeName, string routingKeys)
        {
            DeadLetterExchangeName = deadLetterExchangeName;
            RoutingKeys = routingKeys;
            ExchangeName = exchangeName;
            QueueName = queueName;
            InitializeExchange();
        }

        private void InitializeExchange()
        {
            var deadExchangeParams = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", DeadLetterExchangeName}
            };
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, false, false, null);
            _channel.QueueDeclare(queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: deadExchangeParams);
            _channel.QueueBind(QueueName, ExchangeName, "");
        }

        public void Process(Message message)
        {
             var publishingProperties = _channel.CreateBasicProperties();
             publishingProperties.Expiration = Delay.ToString();
             var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
             _channel.BasicPublish(ExchangeName, RoutingKeys, publishingProperties, body);    
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
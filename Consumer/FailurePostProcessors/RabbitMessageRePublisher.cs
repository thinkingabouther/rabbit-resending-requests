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
        public string BaseQueueName { get; }
        public string DeadLetterExchangeName { get; }
        public string RoutingKeys { get; }
        public int BaseDelay = 2000;
        
        private readonly IModel _channel = new ConnectionFactory().CreateConnection().CreateModel();

        public RabbitMessageRePublisher(string exchangeName, string queueName, string deadLetterExchangeName, string routingKeys)
        {
            DeadLetterExchangeName = deadLetterExchangeName;
            RoutingKeys = routingKeys;
            ExchangeName = exchangeName;
            BaseQueueName = queueName;
            InitializeExchange();
        }

        private void InitializeExchange()
        {
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, false, false, null);
        }

        public void Process(Message message)
        {
            message.RepublishCount++;
            var currentDelay = (int)(Math.Pow(2, message.RepublishCount) * BaseDelay);
            var routingKey = PrepareQueue(currentDelay);
            var publishingProperties = _channel.CreateBasicProperties();
            publishingProperties.Expiration = currentDelay.ToString();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            Console.WriteLine($"Message republished for {message.RepublishCount} time with delay {currentDelay}");
            _channel.BasicPublish(ExchangeName, routingKey, publishingProperties, body);    
        }

        private string PrepareQueue(int delay)
        {
            string queueName = $"{BaseQueueName}.{delay}";
            string routingKey = queueName;
            var deadExchangeParams = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", DeadLetterExchangeName},
                {"x-dead-letter-routing-key", RoutingKeys}
            };
            _channel.QueueDeclare(queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: deadExchangeParams);
            _channel.QueueBind(queueName, ExchangeName, routingKey);
            return routingKey;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
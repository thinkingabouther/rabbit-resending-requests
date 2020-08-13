using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class RabbitMessageConsumer : IDisposable
    {
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }

        public string RetryExchangeName => ExchangeName + ".retry";
        public string RetryQueueName => QueueName + ".retry";

        public IRequester Requester { get; set; }

        public int Delay = 10000;

        private IModel _channel = new ConnectionFactory().CreateConnection().CreateModel();

        public RabbitMessageConsumer(string exchangeName, string queueName, string routingKey, IRequester requester)
        {
            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKey = routingKey;
            Requester = requester;
        }
        private void InitializeExchange()
        {
            var deadExchangeParams = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", ExchangeName}
            };
                    
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueBind(QueueName, ExchangeName, RoutingKey);
                    
            _channel.ExchangeDeclare(RetryExchangeName, ExchangeType.Fanout, false, false, null);
            _channel.QueueDeclare(queue: RetryQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: deadExchangeParams);
            _channel.QueueBind(RetryQueueName, RetryExchangeName, "");
        }

        public void Initialize()
        {
            InitializeExchange();
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, args) =>
            {
                if (!Requester.TryRequest(args))
                {
                    var publishingProperties = _channel.CreateBasicProperties();
                    publishingProperties.Expiration = Delay.ToString();
                    _channel.BasicPublish(exchange: RetryExchangeName , RoutingKey, publishingProperties, args.Body);    
                }
            };
            _channel.BasicConsume(QueueName, true, consumer);
            Console.WriteLine($"Consumer ready! Listening to exchange \"{ExchangeName}\", queue \"{QueueName}\" with routing key \"{RoutingKey}\"");
        }
        
        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
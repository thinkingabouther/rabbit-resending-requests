using System;
using System.Text;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Producer
{
    static class Program
    {
        static string exchangeName = "exchange";
        static string routingKey = "request";
        
        public static void Main(string[] args)
        {
            using var producer = new RabbitMessageProducer(exchangeName, routingKey);
            var message = "https://google.com";
            producer.Publish(message);
            Console.ReadLine();
        }
    }
}
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
            string message;
            if (args.Length != 1)
            {
                Console.WriteLine("You have to pass one argument: URL for consumer. Default will be used");
                message = "https://google.com";
            }
            else
            {
                message = args[0];
            }
            using var producer = new RabbitMessageProducer(exchangeName, routingKey);
            producer.Publish(message);
            Console.ReadLine();
        }
    }
}
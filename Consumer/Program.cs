using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    static class Program
    {
        static string exchangeName = "exchange";
        static string queueName = "queue";
        static string routingKey = "request";

        public static void Main()
        {
            var requester = new GetRequester();
            using var consumer = new RabbitMessageConsumer(exchangeName, queueName, routingKey, requester);
            consumer.Initialize();
            Console.ReadLine();
        }
    }
}
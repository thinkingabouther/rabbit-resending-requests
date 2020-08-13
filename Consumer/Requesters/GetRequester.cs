using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class GetRequester : IRequester
    {
        public bool TryRequest(BasicDeliverEventArgs eventArgs)
        {
            byte[] body = eventArgs.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<Message>(messageString);
            HttpClient client = new HttpClient();
            var responseMessage = client.GetAsync(message.Url).GetAwaiter().GetResult();
            //responseMessage.StatusCode = HttpStatusCode.NotFound; // uncomment the line to make the consumer deny the message 
            if (CheckStatusCode(responseMessage))
            {
                Console.WriteLine(" Message was acknowledged");
                Console.WriteLine(" [Response] {0}", responseMessage);
                return true;
            }
            Console.WriteLine(" Message was acknowledged negatively");
            return false;
        }
        
        private static bool CheckStatusCode(HttpResponseMessage responseMessage)
        {
            var responseSuccessfulPattern = @"20.";
            return Regex.IsMatch(((int)responseMessage.StatusCode).ToString(), responseSuccessfulPattern);
        }
    }
}
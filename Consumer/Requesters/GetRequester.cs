using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Common;
using Consumer.FailurePostProcessors;

namespace Consumer.Requesters
{
    public class GetRequester : IRequester
    {
        public GetRequester(){}
        public GetRequester(IFailurePostProcessor postProcessor)
        {
            PostProcessor = postProcessor;
        }

        private IFailurePostProcessor PostProcessor { get;  }
        public bool TryRequest(Message message)
        {
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
            PostProcessor?.Process(message);
            return false;
        }
        
        private static bool CheckStatusCode(HttpResponseMessage responseMessage)
        {
            var responseSuccessfulPattern = @"20.";
            return Regex.IsMatch(((int)responseMessage.StatusCode).ToString(), responseSuccessfulPattern);
        }
    }
}
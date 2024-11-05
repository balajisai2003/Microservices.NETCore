using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mongo.ServiceBus
{
    public class MessageBus : IMessageBus
    {
        private string _connectionString = "";
        public async Task PublishMessage(string topic_queue_name, string message)
        {
            await using var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(topic_queue_name);
            var JsonMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonMessage)){
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}

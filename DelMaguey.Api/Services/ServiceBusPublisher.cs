using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace DelMaguey.Api.Services
{
    public class ServiceBusPublisher
    {

        private readonly ServiceBusClient _client;
        private readonly string _queueName = "transactions";
        private readonly string topic = "orders-topic";

        public ServiceBusPublisher(IConfiguration config)
        {
            string? cs = config.GetConnectionString("ServiceBusConn");

            if (!string.IsNullOrEmpty(cs))
                _client = new ServiceBusClient(cs);
            else
                throw new ArgumentNullException(nameof(cs), "ServiceBus connection string cannot be null or empty.");
        }

        public async Task SendTransactionAsync(object transaction) 
        {
            var sender = _client.CreateSender(_queueName);

            var message = new ServiceBusMessage(
                JsonSerializer.Serialize(transaction)
                );

            await sender.SendMessageAsync(message);
        }

    }
}

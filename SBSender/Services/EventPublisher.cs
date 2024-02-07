using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace SBSender.Services
{
    public class EventSender
    {
        private readonly ServiceBusSender _sender;

        public EventSender(ServiceBusClient serviceBusClient)
        {
            _sender = serviceBusClient.CreateSender("maziad-saddik");
        }

        public async Task PublishEventAsync(CustomerEvent customerEvent)
        {
            var json = JsonSerializer.Serialize(customerEvent);

            await _sender.SendMessageAsync(new ServiceBusMessage()
            {
                Body = new BinaryData(Encoding.UTF8.GetBytes(json)),
                PartitionKey = customerEvent.Id.ToString(),
                SessionId = customerEvent.Id.ToString(),
                Subject = customerEvent.Type
            });
        }
    }
}

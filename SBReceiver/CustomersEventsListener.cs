
using Azure.Messaging.ServiceBus;
using SBReceiver.Services;
using System.Text;
using System.Text.Json;

namespace Listener
{
    public class CustomersEventsListener : IHostedService
    {
        private readonly ServiceBusSessionProcessor _processor;

        public CustomersEventsListener(ServiceBusClient serviceBusClient)
        {
            _processor = serviceBusClient.CreateSessionProcessor(
                queueName: "maziad-saddik",
                options: new ServiceBusSessionProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    PrefetchCount = 1,
                    MaxConcurrentCallsPerSession = 1,
                    MaxConcurrentSessions = 100,
                });


            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }

        private async Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
        {
            var json = Encoding.UTF8.GetString(arg.Message.Body);

            var customerEvent = JsonSerializer.Deserialize<CustomerEvent>(json);

            Console.WriteLine($"Received event {customerEvent?.Name} for customer {customerEvent?.Id}");

            await arg.CompleteMessageAsync(arg.Message);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _processor.StartProcessingAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _processor.CloseAsync();
        }
    }
}

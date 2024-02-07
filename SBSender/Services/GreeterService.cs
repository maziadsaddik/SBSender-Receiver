using Grpc.Core;
using SBSender;

namespace SBSender.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly EventSender _publisher;

        public GreeterService(ILogger<GreeterService> logger, EventSender publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            await _publisher.PublishEventAsync(new CustomerEvent()
            {
                Id = request.Id,
                Name = request.Name,
                Address = request.Address,
                Type = "CustomerUpdated"
            });

            return new HelloReply
            {
                Message = "Hello " + request.Name
            };
        }
    }
}

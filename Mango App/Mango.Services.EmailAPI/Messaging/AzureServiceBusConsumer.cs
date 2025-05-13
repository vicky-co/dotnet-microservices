using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Services;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;


        public AzureServiceBusConsumer( IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            this.serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            this.emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(this.serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);

        }
    }
}

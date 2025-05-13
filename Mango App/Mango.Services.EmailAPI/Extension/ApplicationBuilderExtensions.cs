using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder builder)
        {
            ServiceBusConsumer = builder.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        }
    }
}

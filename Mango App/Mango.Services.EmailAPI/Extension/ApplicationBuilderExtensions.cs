using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder builder)
        {
            ServiceBusConsumer = builder.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = builder.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnApplicationStart);
            hostApplicationLife.ApplicationStopping.Register(OnApplicationStop);

            return builder;
        }

        private static void OnApplicationStop()
        {
            ServiceBusConsumer.Stop();
        }

        private static void OnApplicationStart()
        {
            ServiceBusConsumer.Start();
        }
    }
}

using EasyNetQ;

namespace Common.Rabbit
{
    public abstract class RabbitMQ
    {
        public readonly IBus _bus;
        protected readonly IServiceProvider _serviceProvider;

        public RabbitMQ(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") != null ? Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") : "host=localhost");

            Configure();
        }

        public abstract void Configure();
    }
}

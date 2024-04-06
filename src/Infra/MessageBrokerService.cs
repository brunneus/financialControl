using Confluent.Kafka;
using System.Text.Json;

namespace FinanceControl.Infra
{
    public interface IMessageBroker
    {
        Task ProduceMessageAsync(object message);
    }

    public class MessageBrokerService : IMessageBroker
    {
        public async Task ProduceMessageAsync(object message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:29092"
            };

            var producer = new ProducerBuilder<Null, string>(config)
                .Build();

            await producer.ProduceAsync("finance.control.events", new Message<Null, string> { Value = JsonSerializer.Serialize(message) });
        }
    }
}

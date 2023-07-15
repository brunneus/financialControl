using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FinanceControl.Infra
{
    public class BrokerOptions
    {
        public string BootstrapServers { get; set; } = string.Empty;
    }

    public class BrokerService
    {
        private readonly IOptions<BrokerOptions> _brokerOptions;

        public BrokerService(IOptions<BrokerOptions> brokerOptions)
        {
            _brokerOptions = brokerOptions;
        }

        public async Task SendMessageAsync(object message, string topic)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _brokerOptions.Value.BootstrapServers,
            };

            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            var kafkaMessage = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message)
            };

            await producer.ProduceAsync(topic, kafkaMessage);
        }
    }
}

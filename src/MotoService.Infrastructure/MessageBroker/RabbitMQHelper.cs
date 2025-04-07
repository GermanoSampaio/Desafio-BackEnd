using RabbitMQ.Client;

namespace MotoService.Infrastructure.MessageBroker
{
    public static class RabbitMqAsyncHelper
    {
        public static async Task<(IConnection connection, IChannel channel)> CreateConnectionAndChannelAsync(string hostName, string userName = "guest", string password = "guest")
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            return (connection, channel);
        }
        public static async Task ConfigureQueueAndExchangeAsync(IChannel channel, string exchangeName, string queueName, string routingKey)
        {
            await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                               type: ExchangeType.Direct,
                                               durable: true,
                                               autoDelete: false);

            await channel.QueueDeclareAsync(queue: queueName,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false);

            await channel.QueueBindAsync(queue: queueName,
                                         exchange: exchangeName,
                                         routingKey: routingKey);
        }
    }
}
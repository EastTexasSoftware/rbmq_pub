using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using static Common.Components.Common;

namespace Consumer
{
    class Program
    {
        private static IConnection _connection;
        private static ConnectionFactory _connectionFactory;
        private static IModel _channel;
        private static EventingBasicConsumer _consumer;

        static void Main(string[] args)
        {
            try
            {
                _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                DeclareExchangeAndBindQueue();

                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };

                _channel.BasicConsume(queue: QueueName,
                                      autoAck: true,
                                      consumer: _consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception}");
            }
            finally
            {
                _connection?.Dispose();
                _channel?.Dispose();
            }
        }

        private static void DeclareExchangeAndBindQueue()
        {
            _channel.ExchangeDeclare(
                   exchange: ExchageName,
                   type: "fanout",
                   durable: false,
                   autoDelete: false,
                   arguments: null);

            _channel.QueueDeclare(queue: QueueName,
                                  durable: false,
                                  exclusive: true,
                                  autoDelete: true,
                                  arguments: null);

            _channel.QueueBind(queue: QueueName,
                               exchange: ExchageName,
                               routingKey: RoutingKey,
                               arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
        }
    }
}

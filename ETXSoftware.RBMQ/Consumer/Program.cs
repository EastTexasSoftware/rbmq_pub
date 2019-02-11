using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using static Common.Components.Common;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory() { HostName = "localhost" };
                using (IConnection connection = connectionFactory.CreateConnection())
                using (IModel channel = connection.CreateModel())
                {
                    EventingBasicConsumer consumer;

                    channel.ExchangeDeclare(
                    exchange: ExchageName,
                    type: "fanout",
                    durable: false,
                    autoDelete: false,
                    arguments: null);

                    channel.QueueDeclare(queue: QueueName,
                                        durable: false,
                                        exclusive: true,
                                        autoDelete: true,
                                        arguments: null);

                    channel.QueueBind(queue: QueueName,
                                        exchange: ExchageName,
                                        routingKey: RoutingKey,
                                        arguments: null);

                    consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };

                    channel.BasicConsume(queue: QueueName,
                                          autoAck: true,
                                          consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception}");
            }
        }
    }
}

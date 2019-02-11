using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Common.Components.Common;

namespace Producer
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
                    channel.ExchangeDeclare(
                        exchange: ExchageName,
                        type: "fanout",
                        durable: false,
                        autoDelete: false,
                        arguments: null);

                    new Task(() =>
                    {
                        int count = 0;

                        while (true)
                        {
                            try
                            {
                                string message = $"Hello World {count++}!";
                                byte[] body = Encoding.UTF8.GetBytes(message);

                                channel.BasicPublish(exchange: ExchageName,
                                                     routingKey: RoutingKey,
                                                     basicProperties: null,
                                                     body: body);

                                Console.WriteLine(" [x] Sent {0}", message);

                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine($"Exception: {exception}");
                            }
                            using (ManualResetEvent mre = new ManualResetEvent(false))
                                mre.WaitOne(TimeSpan.FromSeconds(1));
                        }
                    }, TaskCreationOptions.LongRunning).Start();

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

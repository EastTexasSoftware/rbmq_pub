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
        private static IConnection _connection;
        private static ConnectionFactory _connectionFactory;
        private static IModel _channel;

        static void Main(string[] args)
        {
            try
            {
                _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: ExchageName,
                    type: "fanout",
                    durable: false,
                    autoDelete: false,
                    arguments: null);

                new Task(() => PublishMessage(_channel), TaskCreationOptions.LongRunning).Start();

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception}");
            }
            finally
            {
                _connection.Dispose();
                _channel.Dispose();
            }
        }

        internal static void PublishMessage(IModel channel)
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
        }
    }
}

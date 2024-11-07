using RabbitMQ.Client;
using System.Diagnostics.Metrics;
using System.Text;

namespace Publisher.Notification
{
    public class Notification
    {
        private static int counter = 0;
        private static Random rnd = new Random();
        private static readonly List<string> notificationText = new List<string> { "Server software updated...", "Cyber attack repelled...", "Exchange rates updated..." };


        private static string GenerateNotificationText()

        {

            return $"{notificationText[rnd.Next(0, 3)]}";

        }






        public Func<Task> CreateNotificationTask(int DelayTime)


        {

            return async () =>

            {

                do
                {
                    await Task.Delay(DelayTime);
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {


                        /*

                        // Установка prefetch count в 10 для очереди 'St_stream_queue'
                        channel.BasicQos(0, 10, false);


                        // Аргументы для потока
                        var streamArgs = new Dictionary<string, object>

                        {
                             { "x-queue-type", "stream" },
                             { "max-length", 1000 },
                             { "message-ttl", TimeSpan.FromMinutes(30).TotalMilliseconds }

                        };

                        */


                        channel.ExchangeDeclare(exchange: "not_stream_notification_exchange", type: ExchangeType.Fanout);
                        channel.QueueDeclare(queue: "not_stream_queue",
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);





                        string notificationText = GenerateNotificationText();

                        string message = $"Notification: {notificationText} winth ID {counter++}";

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(
                            exchange: "not_stream_notification_exchange",
                            routingKey: "",
                            basicProperties: null,
                            body: body);




                        Console.WriteLine($"Notification |{notificationText}| is sent into Fanout Exchange");



                    }


                } while (true);






            };




        }








    }
}
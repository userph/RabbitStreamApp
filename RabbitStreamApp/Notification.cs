using RabbitMQ.Client;
using System.Diagnostics.Metrics;
using System.Text;

namespace Publisher.Notification
{
    public class Notification
    {
        private static int counter = 1;
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


                       

                        // Установка prefetch count в 10 для очереди 'St_stream_queue'
                       // channel.BasicQos(0, 10, false);


                        // Аргументы для потока
                        var quorumArgs = new Dictionary<string, object>

                        {
                             { "x-queue-type", "quorum" },
                             { "x-delivery-limit", 5 }
                            

                        };

                        


                        channel.ExchangeDeclare(exchange: "quorum_exchange", type: ExchangeType.Fanout);
                        channel.QueueDeclare(queue: "quorum_queue",
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: quorumArgs);





                        string notificationText = GenerateNotificationText() + $"winth ID { counter++}";

                        string message = $"Notification: {notificationText}";

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(
                            exchange: "quorum_exchange",
                            routingKey: "",
                            basicProperties: null,
                            body: body);




                        Console.WriteLine($"Notification |{notificationText}| is sent into Exchange");



                    }


                } while (true);






            };




        }








    }
}
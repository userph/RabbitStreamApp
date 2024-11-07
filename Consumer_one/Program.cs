using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace Consumer.One

{


    class Program
    {

        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())

            {

                


                // Аргументы для потока
                var streamArgs = new Dictionary<string, object>

                        {
                             { "x-queue-type", "stream" },
                             { "max-length", 1000 },
                             { "message-ttl", TimeSpan.FromMinutes(30).TotalMilliseconds }

                        };

                

                channel.ExchangeDeclare(exchange: "S_notification_exchange", type: ExchangeType.Fanout);
                channel.QueueDeclare(queue: "S_stream_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "S_stream_queue",
                  exchange: "S_notification_exchange",
                  routingKey: string.Empty);

                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("Received message:" + message);

                };


           //     channel.BasicQos(0, 1, false); // Установка prefetch count равным 1

                channel.BasicConsume(queue: "S_stream_queue",
                autoAck: true,
                consumer: consumer);

                Console.ReadLine();


            }






            }


    }


}



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


                // Установка prefetch count в 10 для очереди 'St_stream_queue'
                //   channel.BasicQos(0, 10, false);


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



                channel.QueueBind(queue: "quorum_queue",
                  exchange: "quorum_exchange",
                  routingKey: string.Empty);

                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("Received " + message);

                };


                //     channel.BasicQos(0, 1, false); // Установка prefetch count равным 1

                channel.BasicConsume(queue: "quorum_queue",
                autoAck: true,
                consumer: consumer);

                Console.ReadLine();


            }
        }
    }
}



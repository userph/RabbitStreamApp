using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace Consumer.Two

{


    class Program
    {

        static void Main(string[] args)
        {

            static int GetID(string phrase)
            {
                // Удаляем лишние пробелы и разбиваем строку на слова
                string[] words = phrase.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Проверяем, есть ли слова, и возвращаем последнее
                return int.Parse(words[^1]);
            }



            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())

            {

                // Аргументы для quorum
                var quorumArgs = new Dictionary<string, object>

                        {
                             { "x-queue-type", "quorum" },
                             { "x-delivery-limit", 1 }


                        };




                channel.ExchangeDeclare(exchange: "quorum_exchange", type: ExchangeType.Fanout);
                channel.QueueDeclare(queue: "Q_quorum_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: quorumArgs);

         

                channel.QueueBind(queue: "Q_quorum_queue",
                  exchange: "quorum_exchange",
                  routingKey: string.Empty);

                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    int messId = GetID(message);



                    if (messId % 3 == 0)
                    {


                        Console.WriteLine($"Rejecting {message} with DeliveryTag {e.DeliveryTag} and Id {messId}");
                        //  channel.BasicReject(e.DeliveryTag, requeue: true);
                        channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);

                    }

                    else

                    {
                        Console.WriteLine("Received " + message + " with DeliveryTag " + e.DeliveryTag + " and Id " + messId);

                        channel.BasicAck(e.DeliveryTag, multiple: false);

                    }


                     



                };


                //     channel.BasicQos(0, 1, false); // Установка prefetch count равным 1

                channel.BasicConsume(queue: "Q_quorum_queue",
                autoAck: false,
                consumer: consumer);

                Console.ReadLine();


            }
        }
    }
}



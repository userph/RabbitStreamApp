using RabbitMQ.Client;
using System.Text;


namespace Publisher.Notification

{
    class Program
    {
        static async Task Main(string[] args)
        {
            var notification = new Notification();
            var commonNotificationTask = notification.CreateNotificationTask(5000);




            var tasks = new List<Task>
            {
                commonNotificationTask()
            };

            await Task.WhenAll(tasks);



            Console.ReadLine();
        }
    }
}
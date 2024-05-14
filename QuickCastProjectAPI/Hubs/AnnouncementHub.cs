using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace QuickCastProjectAPI.Hubs
{
    public class AnnouncementHub : Hub
    {
        private readonly ConnectionFactory _connectionFactory;

        public AnnouncementHub(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public override async Task OnConnectedAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "announcement_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var articleJson = Encoding.UTF8.GetString(body);
                };
                channel.BasicConsume(queue: "announcement_queue",
                                      autoAck: true,
                                      consumer: consumer);

                await base.OnConnectedAsync();
            }
        }




    }
}

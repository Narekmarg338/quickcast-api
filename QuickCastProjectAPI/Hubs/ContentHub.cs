using Microsoft.AspNetCore.SignalR;
using QuickCastProjectAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QuickCastProjectAPI.Hubs
{
    public class ContentHub : Hub
    {
        private readonly ConnectionFactory _connectionFactory;

        public ContentHub(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public override async Task OnConnectedAsync()
        { 
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "content_queue",
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
                channel.BasicConsume(queue: "content_queue",
                                      autoAck: true,
                                      consumer: consumer);

                await base.OnConnectedAsync();
            }
        }

 

     
    }
}

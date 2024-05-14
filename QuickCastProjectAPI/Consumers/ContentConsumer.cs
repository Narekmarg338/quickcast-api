using Microsoft.AspNetCore.SignalR;
using QuickCastProjectAPI.Hubs;
using QuickCastProjectAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace QuickCastProjectAPI.Consumers
{


    public class ContentConsumer
    {
        private readonly IHubContext<ContentHub> _hubContext;
        private readonly ConnectionFactory _connectionFactory;

        public ContentConsumer(IHubContext<ContentHub> hubContext, ConnectionFactory connectionFactory)
        {
            _hubContext = hubContext;
            _connectionFactory = connectionFactory;
        }
     
 
        public void StartConsuming(CancellationToken cancellationToken)
        {
            Task.Run(() =>
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
                        var contentJson = Encoding.UTF8.GetString(body);

                        var content = JsonSerializer.Deserialize<Content>(contentJson);
                        if (content != null)
                        {
                            if (content.TargetAudience.Contains("All Employees"))
                            {
                                await _hubContext.Clients.All.SendAsync("ReceiveContent", content);
                            }
                            else
                            {
                                foreach (var group in content.TargetAudience)
                                {
                                    await _hubContext.Clients.Group(group).SendAsync("ReceiveContent", content);
                                }
                            }   

                        
                         
                        }
                   
                    };

                    channel.BasicConsume(queue: "content_queue",
                                          autoAck: true,
                                          consumer: consumer);

                    
                    while (true)
                    {
                        Thread.Sleep(1000); 
                    }
                }
            }, cancellationToken);
        }
    }


}   

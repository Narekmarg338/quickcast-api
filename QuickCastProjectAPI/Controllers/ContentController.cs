using Microsoft.AspNetCore.Mvc;
using QuickCastProjectAPI.Models;
using RabbitMQ.Client;
using System;
using System.Text;

[ApiController]
[Route("[controller]")]
public class ContentController : ControllerBase
{
    private readonly ConnectionFactory _connectionFactory;

    public ContentController(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpPost]
    public IActionResult PostContent([FromBody] Content content)
    {
        try
        {  
            var articleJson = System.Text.Json.JsonSerializer.Serialize(content);

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "content_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(articleJson);
                channel.BasicPublish(exchange: "",
                                      routingKey: "content_queue",
                                      basicProperties: null,
                                      body: body);
            }
            return Ok("Article uploaded successfully!");

                }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading article: {ex.Message} {ex.GetType()}");
        }
    }
}

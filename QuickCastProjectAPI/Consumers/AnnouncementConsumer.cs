using Microsoft.AspNetCore.SignalR;
using QuickCastProjectAPI.Hubs;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using QuickCastProjectAPI.Models;

namespace QuickCastProjectAPI.Consumers
{
    public class AnnouncementConsumer
    {

        private readonly IHubContext<AnnouncementHub> _hubContext;
        private readonly ConnectionFactory _connectionFactory;

        public AnnouncementConsumer(IHubContext<AnnouncementHub> hubContext, ConnectionFactory connectionFactory)
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
                    channel.QueueDeclare(queue: "announcement_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var announcementJson = Encoding.UTF8.GetString(body);


                        var announcement = JsonSerializer.Deserialize<Announcement>(announcementJson);


                        await _hubContext.Clients.All.SendAsync("Recieve Announcement", announcement);
                    };
                    channel.BasicConsume(queue: "announcement_queue",
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


// Role: Author 

// In web admin we create an article for Managers 
// Article is saved to database 
// Message is added to rabbit mq that mentions the article ID 
// SignalR server reads the message from rabbit mq 
// SignlarR servers queries the database by article ID 
// Article has configuration, for example, the user that are allowed to view the article

// SignalR gets the article content and configuration. 
// Based on configuration determine which users are eligible to view the article 
// For all eligible users pushes update notification to desktop clients
// Desktop clients receive the notification and request the new article from the 
// server 


// Two configurations: 
// For each piece of content there is configuration of users that are able to see it.


// Integration with Active Directory. 
// Azure AD 
// Our user is logged in to Azure AD
// When they open the desktop app, the app requests an auth token from the Azure AD server
// The desktop sends the token to the SignalR server
// Signlar connects to Azure AD and verifies the token. 
// When SignalR verifies the token it also gets a set of claims about the user. 
// The set of claims contains the user role in active directory. 

// I am a regular user not a manager. 
// I can change my role locally and lie to SignalR

// Change preferences which are not part of master data
// Master data is still coming from Azure AD or HR system?

// Assume that each piece of content has an expiration period. 

// Two cases
// Startup: 
// During start up get all relevant content and filter out expired ones 

// Real time: 
// Do we need to update real-time? 



//UI EXACTLY LIKE FIGMA
//		USER PREFRENCES	 (DEMO WITH MULTIPLE USER OF DIFFERNT ROLES/LOCATIONS)
//		DISPLAY LOGIC
//			SHOW ARTCILES AS PER PRIOIRTY(HIGH - MIDEUM - LOW) / DATE ORDER			
//		LOGIC OF EXPIRED NEWS
			 
			
		
//		HOST ON AZURE
//		BROADCASE USING SWAGGER
//		DISPLAY REALTIME ON APP

// multiple users
// create separate exe files 
// for each user 

// creating on exe file 
// after install change ini file 
// all config in ini file 
// for switching users we can change the config 


// only show one user pref icon in toolbar
// fix announcement height 
// use colors for article priority, red,yellow, green 
// fix ceo news width and alignment 
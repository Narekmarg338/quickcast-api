using QuickCastProjectAPI.Consumers;
using QuickCastProjectAPI.Hubs;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ContentConsumer>();
builder.Services.AddSingleton<ConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,         
        UserName = "guest",     
        Password = "guest",  
        VirtualHost = "/"    
    };

    return factory;
});
var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var articleConsumer = app.Services.GetRequiredService<ContentConsumer>();
var cancellationTokenSource = new CancellationTokenSource();
articleConsumer.StartConsuming(cancellationTokenSource.Token);
app.UseAuthorization(); 
app.UseRouting();
app.MapHub<ContentHub>("/contenthub");
app.MapControllers();

app.Run();

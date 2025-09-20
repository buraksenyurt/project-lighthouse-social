using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "admin",
    Password = "admin1234",
    VirtualHost = "/"
};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "lighthouse-project-events",
    type: ExchangeType.Topic,
    durable: true,
    autoDelete: false);

await channel.QueueDeclareAsync(
    queue: "lighthouse-project-queue",
    durable: true,
    exclusive: false,
    autoDelete: false);

await channel.QueueBindAsync(
    queue: "lighthouse-project-queue",
    exchange: "lighthouse-project-events",
    routingKey: "lighthouse.*");

Console.WriteLine("Waiting for messages. Press any key to exit.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;

    Console.WriteLine($"[x] Received '{routingKey}': {message}\n");

    await Task.CompletedTask;
};

await channel.BasicConsumeAsync("lighthouse-project-queue", autoAck: true, consumer: consumer);

Console.ReadLine();
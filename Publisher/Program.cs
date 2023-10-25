using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
    channel.QueueDeclare(
        queue: "first",
        exclusive: false,
        durable: true,
        autoDelete: false,
        arguments: null);

channel.QueueDeclare(
    queue: "second",
    exclusive: false,
    durable: true,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received in second: {message}");
};

channel.BasicConsume(queue: "second", autoAck: true, consumer: consumer);

while (true)
{
    Console.WriteLine("Введите сообщение:");
    var message = Console.ReadLine();
    var body = Encoding.UTF8.GetBytes(message);
        
    channel.BasicPublish(exchange: "", routingKey: "first", basicProperties: null, body: body);
}
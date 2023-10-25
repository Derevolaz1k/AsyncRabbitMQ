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
    Console.WriteLine($"Received in first: {message}");

    var responseMessage = "Сообщение доставлено.";
    var responseBody = Encoding.UTF8.GetBytes(responseMessage);
    Thread.Sleep(10000);
    channel.BasicPublish(exchange: "", routingKey: "second", basicProperties: null, body: responseBody);
};

channel.BasicConsume(queue: "first", autoAck: true, consumer: consumer);

Console.WriteLine("Нажмите [Enter], чтобы выйти.");
Console.ReadLine();
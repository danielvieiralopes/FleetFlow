using FleetFlow.Application.Interfaces;
using RabbitMQ.Client;

namespace FleetFlow.Infrastructure.Messaging;

public class RabbitMqService : IMessageBus
{
    private readonly IConnection _connection;

    public RabbitMqService(IConnection connection)
    {
        _connection = connection;
    }

    public void Publish(string queue, byte[] message)
    {
        using var channel = _connection.CreateModel();

        // Garante que a fila exista.
        channel.QueueDeclare(queue: queue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        // Publica a mensagem.
        channel.BasicPublish(exchange: "",
                             routingKey: queue,
                             basicProperties: null,
                             body: message);
    }
}
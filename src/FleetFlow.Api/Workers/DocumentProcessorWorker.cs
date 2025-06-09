using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FleetFlow.Api.Workers;

public class DocumentProcessorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    private readonly string _queueName;

    public DocumentProcessorWorker(IServiceProvider serviceProvider, IConnection connection, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _configuration = configuration;
        _queueName = _configuration["RabbitMqSettings:QueueName"]!;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var documentInfo = JsonSerializer.Deserialize<DocumentUploadMessage>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (documentInfo is not null)
            {
                // Criamos um novo escopo para resolver serviços, pois o worker é um singleton.
                using var scope = _serviceProvider.CreateScope();
                var documentRepository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var minioEndpoint = _configuration["MinioSettings:Endpoint"];
                var bucketName = _configuration["MinioSettings:BucketName"];
                var fileAccessUrl = $"http://{minioEndpoint}/{bucketName}/{documentInfo.FileUrl}";

                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    VehicleId = documentInfo.VehicleId,
                    FileName = documentInfo.FileName,
                    FileMimeType = documentInfo.FileMimeType,
                    FileUrl = fileAccessUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await documentRepository.AddAsync(document);
                await unitOfWork.SaveChangesAsync(stoppingToken);
            }

            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    // Classe DTO auxiliar para deserializar a mensagem
    private record DocumentUploadMessage(Guid VehicleId, string FileName, string FileMimeType, string FileUrl);
}
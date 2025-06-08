using FleetFlow.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public class RequestUploadCommandHandler : IRequestHandler<RequestUploadCommand, RequestUploadResult>
{
    private readonly IStorageService _storageService;
    private readonly IMessageBus _messageBus;
    private readonly IConfiguration _configuration;

    public RequestUploadCommandHandler(IStorageService storageService, IMessageBus messageBus, IConfiguration configuration)
    {
        _storageService = storageService;
        _messageBus = messageBus;
        _configuration = configuration;
    }

    public async Task<RequestUploadResult> Handle(RequestUploadCommand request, CancellationToken cancellationToken)
    {
        var bucketName = _configuration["MinioSettings:BucketName"]!;
        // Cria um nome de objeto único para evitar colisões
        var objectName = $"{request.VehicleId}/{Guid.NewGuid()}-{request.FileName}";

        // 1. Gera a URL de upload pré-assinada
        var uploadUrl = await _storageService.GeneratePresignedUploadUrlAsync(bucketName, objectName);

        // 2. Publica a mensagem na fila para processamento assíncrono
        var message = new
        {
            VehicleId = request.VehicleId,
            FileName = request.FileName,
            FileMimeType = request.FileMimeType,
            FileUrl = objectName // Enviamos o objectName/key para o worker
        };
        var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _messageBus.Publish(_configuration["RabbitMqSettings:QueueName"]!, messageBody);

        // 3. Retorna a URL para o cliente
        return new RequestUploadResult(request.FileName, request.FileMimeType, uploadUrl);
    }
}
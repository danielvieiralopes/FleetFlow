using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public record RequestUploadCommand(Guid VehicleId, string FileName, string FileMimeType) : IRequest<RequestUploadResult>;
public record RequestUploadResult(string FileName, string FileMimeType, string UploadUrl);
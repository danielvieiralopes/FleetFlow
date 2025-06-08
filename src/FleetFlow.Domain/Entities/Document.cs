namespace FleetFlow.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileMimeType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty; // URL de acesso ao arquivo no MinIO
    public DateTime CreatedAt { get; set; }
}
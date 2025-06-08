namespace FleetFlow.Application.Interfaces;

public interface IStorageService
{
    Task<string> GeneratePresignedUploadUrlAsync(string bucketName, string objectName);
}
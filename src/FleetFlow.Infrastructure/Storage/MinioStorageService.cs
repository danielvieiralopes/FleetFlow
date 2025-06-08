using FleetFlow.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace FleetFlow.Infrastructure.Storage;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;

    public MinioStorageService(IMinioClient minioClient, IConfiguration configuration)
    {
        _minioClient = minioClient;
        _configuration = configuration;
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(string bucketName, string objectName)
    {
        // Verifica se o bucket existe, se não, cria.
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs);
        }

        // Gera a URL pré-assinada para um método PUT com validade de 5 minutos.
        var args = new PresignedPutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(60 * 5); // 5 minutos

        return await _minioClient.PresignedPutObjectAsync(args);
    }
}
using System;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;

namespace Tfo.UserService.Business.MinIO;

class BucketCommand
{
    public string _bucketName;
    public string _objectName;
    public string _filePath;
    public string _contentType;

    public BucketCommand(string bucketName, string objectName, string filePath, string contentType)
    {
        _bucketName = bucketName;
        _objectName = objectName;
        _filePath = filePath;
        _contentType = contentType;
    }
}

public class MinIOUpload
{
    bool secure = true;
    private readonly MinioClient _client;
    private BucketCommand _bucketCommand;
    
    public MinIOUpload(string domainName, string accessKey, string secretKey, string bucketName, string objectName, string filePath, string contentType)
    {
        _client = new MinioClient()
            .WithEndpoint(domainName)
            .WithCredentials(accessKey, secretKey)
            .Build();

        _bucketCommand = new BucketCommand(bucketName, objectName, filePath, contentType);
    }
    
    public void Execute()
    {
        Run(_client, _bucketCommand).Wait();
    }
    private static async Task Run(MinioClient client, BucketCommand bucketCommand)
    {
        try
        {
            // Make a bucket on the server, if not already present.
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketCommand._bucketName);
            bool found = await client.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketCommand._bucketName);
                await client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
            // Upload a file to bucket.
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketCommand._bucketName)
                .WithObject(bucketCommand._objectName)
                .WithFileName(bucketCommand._filePath)
                .WithContentType(bucketCommand._contentType);
            await client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            Console.WriteLine("Successfully uploaded " + bucketCommand._objectName );
        }
        catch (MinioException e)
        {
            Console.WriteLine("File Upload Error: {0}", e.Message);
        }
    }
}
using System;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;

namespace Tfo.UserService.Business.MinIO;

public class MinIODownload
{
    
    private readonly MinioClient _client;

    public MinIODownload(string domainName, string accessKey, string secretKey)
    {
        _client = new MinioClient()
            .WithEndpoint(domainName)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }
    
    public object Execute(string bucketName, string filePath, string objectName)
    {
        return Run(_client, bucketName, filePath, objectName );
    }
    
    private static async Task<object> Run(MinioClient client, string bucketName, string filePath, string objectName)
    {
        object result = null;
        try
        {
            // Download a file from bucket.
            var getObjectArg = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFile(filePath) ;
            result = await client.GetObjectAsync(getObjectArg).ConfigureAwait(false);
            Console.WriteLine("Successfully downloaded " + objectName );
            
        }
        catch (MinioException e)
        {
            Console.WriteLine("File Upload Error: {0}", e.Message);
        }
        return result;
    }
}
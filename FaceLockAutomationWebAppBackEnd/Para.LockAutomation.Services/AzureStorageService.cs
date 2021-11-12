using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Para.LockAutomation.Models;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Service
{
    public class AzureStorageService
    {
        private AzureStorageConfig _storageConfig;

        public AzureStorageService(IOptions<AzureStorageConfig> aadConfig)
        {
            _storageConfig = aadConfig.Value;
        }

        public async Task<AzureFile> UploadFileToBlobStorage(string containerName, string fileDir, IFormFile uploadFile, bool isPrivate = false)
        {
            CloudBlobContainer container = await GetBlobContainer(containerName, isPrivate);

            string blockBlobName = $"{fileDir}.{uploadFile.FileName.Split('.').LastOrDefault()}";

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(blockBlobName);
            var uri = container.Uri.AbsoluteUri;

            using (var fileStream = uploadFile.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return new AzureFile
            {
                FileName = blockBlobName.Split('/').Last(),
                FilePath = blockBlobName,
                Uri = uri + '/' + blockBlobName
            };
        }

        public async Task<CloudBlobContainer> GetBlobContainer(string containerName, bool isPrivate = false)
        {

            StorageCredentials storageCredentials = new StorageCredentials(_storageConfig.AccountName, _storageConfig.AccessKey);
            CloudStorageAccount storageAcc = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient client = storageAcc.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);

            if (! await container.ExistsAsync())
            {
                await container.CreateAsync();

                if (isPrivate)
                {
                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off
                    });
                }
                else
                {
                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                }
            }

            return container;
        }

        public async Task DeleteBlobFile(string containerName, string fileDir, bool isPrivate)
        {
            CloudBlobContainer container = await GetBlobContainer(containerName, isPrivate);
            var blob = container.GetBlockBlobReference(fileDir);

            if (!blob.IsDeleted)
            {
                await blob.DeleteAsync();
            }
        }
    }
}

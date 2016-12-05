using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IoTLab.AzureStorageTraining.BlobStorageSample
{
    public class SamplesBase
    {

        public CloudBlobClient SetupClient(string accountName, string key)
        {
            StorageCredentials creadentials = new StorageCredentials(accountName, key);

            CloudStorageAccount storageAccount = new CloudStorageAccount(creadentials, true);

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            return client;
        }

        public CloudBlobContainer SetupContainer(CloudBlobClient client, string containerName)
        {
            CloudBlobContainer container = client.GetContainerReference(containerName);
            if (container.CreateIfNotExists())
            {
                container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            return container;
        }



        public static string GetSharedAccessSignatureForBlob(CloudBlob blob)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            return blob.Uri + sasBlobToken;
        }
    }
}

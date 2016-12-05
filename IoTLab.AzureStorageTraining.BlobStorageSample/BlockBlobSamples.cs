using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoTLab.AzureStorageTraining.BlobStorageSample
{
    public class BlockBlobSamples : SamplesBase
    {

        public string UploadData(CloudBlobContainer container, string key, byte[] data)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);
            blockBlob.StreamWriteSizeInBytes = 256 * 1024; //256 k
            TimeSpan backOffPeriod = TimeSpan.FromSeconds(2);
            int retryCount = 1;
            BlobRequestOptions bro = new BlobRequestOptions()
            {
                SingleBlobUploadThresholdInBytes = 1024 * 1024, //1MB, the minimum
                ParallelOperationThreadCount = 1,
                RetryPolicy = new ExponentialRetry(backOffPeriod, retryCount),
            };
            blockBlob.UploadFromByteArray(data, 0, data.Length);
            return GetSharedAccessSignatureForBlob(blockBlob);
        }

        public string UploadText(CloudBlobContainer container, string key, string text)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);
            blockBlob.UploadText(text);
            return GetSharedAccessSignatureForBlob(blockBlob);
        }

        public byte[] DowloadData(CloudBlobContainer container, string key)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);
            MemoryStream stream = new MemoryStream();
            blockBlob.DownloadToStream(stream);
            var data = stream.ToArray();
            return data;
        }

        public string DownloadText(CloudBlobContainer container, string key)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);
            return blockBlob.DownloadText();
        }

        public string UploadAsBlocksList(CloudBlobContainer container, string key, byte[] data)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(key);
            blockBlob.Delete();
            var partitionSize = 1 * 1028;
            var partNumber = 0;
            List<string> blocksIds = new List<string>();
            for (int i = 0; i < data.Length; i+=partitionSize)
            {
                var bufferSize = Math.Min(512, data.Length - i);
                var buffer = new byte[bufferSize];
                Array.Copy(data,i,buffer,0,buffer.Length);

                string blockId =
                  Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("BlockId{0}",
                    partNumber.ToString("0000000"))));
                blocksIds.Add(blockId);

                blockBlob.PutBlock(blockId, new MemoryStream(buffer), null);

                partNumber++;
            }

            blockBlob.PutBlockList(blocksIds);
            return GetSharedAccessSignatureForBlob(blockBlob);
        }
    }

    public static class HashExtentions
    {
        public static string ToHex(this byte[] bytes, bool upperCase = false)
        {
            var result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(i.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }
}


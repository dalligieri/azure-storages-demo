using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IoTLab.AzureStorageTraining.BlobStorageSample
{
    public class PageBlobSamples : SamplesBase
    {
        public string UploadData(CloudBlobContainer container, string key, byte[] data)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(key);
            pageBlob.UploadFromByteArray(data, 0, data.Length);
            return GetSharedAccessSignatureForBlob(pageBlob);
        }

        public byte[] DownloadData(CloudBlobContainer container, string key)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(key);
            MemoryStream stream = new MemoryStream();
            pageBlob.DownloadToStream(stream);
            var data = stream.ToArray();
            return data;
        }

        public string UploadPageData(CloudBlobContainer container, string key, Stream data, int startOffset)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(key);
            pageBlob.WritePages(data, startOffset);
            return GetSharedAccessSignatureForBlob(pageBlob);
        }

        public byte[] DownloadPageData(CloudBlobContainer container, string key, long startOffset, long length)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(key);
            byte[] result = new byte[length];
            pageBlob.DownloadRangeToByteArray(result,0, startOffset, length);
            return result;
        }
    }
}

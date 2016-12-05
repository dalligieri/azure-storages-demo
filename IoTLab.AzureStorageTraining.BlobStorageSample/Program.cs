using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLab.AzureStorageTraining.BlobStorageSample
{
    //https://github.com/dalligieri/azure-storages-demo
    class Program
    {
        static void Main(string[] args)
        {
            var accountName = "iotlabdemo2016";
            var key = "MDwybITcIvLo03K1+gyPsncBP9Vma9C9XmV43V8Noy7U748o+WNB56DRhYfxEmdZnVSsCSIpUZlAkm44YAFLrQ==";
            BlockBlobSamples samples = new BlockBlobSamples();

            var client = samples.SetupClient(accountName, key);
            var container = samples.SetupContainer(client, "image");

            var data = File.ReadAllBytes("azure-logo.jpg");

            //var sas = samples.UploadAsBlocksList(container, "azure-logo.jpg", data);
            //var blobs = container.ListBlobs().ToArray();

            var newSize = (int) (data.Length/512);
            if (newSize*512 < data.Length)
            {
                newSize += 1;
            }

            var buffer = new byte[newSize*512];
            Array.Copy(data,buffer, data.Length);

            var pageSamples = new PageBlobSamples();
            pageSamples.UploadData(container, "page-logo.jpg", buffer);
            //var stream = new MemoryStream(data);
            //pageSamples.UploadPageData(container, "page-logo.jpg", stream, 1000*1024);
        }
    }
}

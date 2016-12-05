using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLab.AzureStorageTraining.BlobStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountName = "iotlabdemo2016";
            var key = "MDwybITcIvLo03K1+gyPsncBP9Vma9C9XmV43V8Noy7U748o+WNB56DRhYfxEmdZnVSsCSIpUZlAkm44YAFLrQ==";
            BlockBlobSamples samples = new BlockBlobSamples();

            var client = samples.SetupClient(accountName, key);
            var container = samples.SetupContainer(client, "test");

            var data = File.ReadAllBytes("azure-logo.jpg");

            var sas = samples.UploadAsBlocksList(container, "azure.jpg", data);
        }
    }
}

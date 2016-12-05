using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace IoTLab.AzureStorageTraining.QueueSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountName = "iotlabdemo2016";
            var key = "MDwybITcIvLo03K1+gyPsncBP9Vma9C9XmV43V8Noy7U748o+WNB56DRhYfxEmdZnVSsCSIpUZlAkm44YAFLrQ==";

            var queue = SetupQueue(accountName, key);

            for (int i = 0; i < 10; i++)
            {
                DeviceMetric dm = new DeviceMetric()
                {
                    CreateDt = DateTime.UtcNow,
                    Name = "Temperature",
                    Value = i.ToString()
                };
                Thread.Sleep(1000);
                SendMetric(queue, dm);
            }

            DeviceMetric metric=null;
            while ((metric = ReceiveMetric(queue))!=null)
            {
                Console.WriteLine("{2:dd.MM.yy hh:mm:ss}: Received metric {0} with value {1}", metric.Name, metric.Value, metric.CreateDt);
            }

            Console.ReadLine();
        }

        static CloudQueue SetupQueue(string accountName, string key)
        {
            StorageCredentials creadentials = new StorageCredentials(accountName, key);

            CloudStorageAccount storageAccount = new CloudStorageAccount(creadentials, true);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            var queueName = "metrics";

            CloudQueue queue = queueClient.GetQueueReference(queueName);

            queue.CreateIfNotExists();
            Console.WriteLine("Queue created");

            return queue;
        }

        static void SendMetric(CloudQueue queue, DeviceMetric metric)
        {
            var content = JsonConvert.SerializeObject(metric);
            CloudQueueMessage message = new CloudQueueMessage(content);
            queue.AddMessage(message);
        }

        static DeviceMetric ReceiveMetric(CloudQueue queue)
        {
            var message = queue.GetMessage(TimeSpan.FromSeconds(20));
            if (message != null)
            {
                var content = message.AsString;
                var metric = JsonConvert.DeserializeObject<DeviceMetric>(content);
                queue.DeleteMessage(message);
                return metric;
            }
            return null;
        }
    }
}

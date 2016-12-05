using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        static CloudQueue SetupQueue(string accountName, string key)
        {
            StorageCredentials creadentials = new StorageCredentials(accountName, key);

            CloudStorageAccount storageAccount = new CloudStorageAccount(creadentials, true);

            CloudQueueClient tableClient = storageAccount.CreateCloudQueueClient();

            var queueName = "Metrics";

            CloudQueue queue = tableClient.GetQueueReference(queueName);

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

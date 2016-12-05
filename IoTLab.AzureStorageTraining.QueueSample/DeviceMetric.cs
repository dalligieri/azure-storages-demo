using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLab.AzureStorageTraining.QueueSample
{
    public class DeviceMetric
    {
        public DateTime CreateDt { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }
}

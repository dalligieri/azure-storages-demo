using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IoTLab.AzureDocumentDbTraining.Sample
{
    public class Device
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<DeviceMetric> DeviceMetrics { get; set; }
    }
}

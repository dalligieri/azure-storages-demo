using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoTLab.AzureStorageTraining.TablesBasicSample
{
    public class Product : TableEntity
    {
        [IgnoreProperty]
        public string Category
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
        [IgnoreProperty]
        public string Name
        {
            get { return RowKey; }
            set { RowKey = value;}
        }

        public double Price { get; set; }
    }
}

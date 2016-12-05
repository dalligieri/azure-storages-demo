using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoTLab.AzureStorageTraining.TablesBasicSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountName = "";
            var key = "";

            CloudTable table = SetupTable(accountName, key);

            Product plum = new Product()
            {
                Category = "Fruits",
                Name = "Plum",
                Price = 1.5
            };

            Product mango = new Product()
            {
                Category = "Fruits",
                Name = "Mango",
                Price = 5
            };

            Product potato = new Product()
            {
                Category = "Vegetables",
                Name = "Potato",
                Price = 2
            };

            SaveProduct(table, plum);
            SaveProduct(table, mango);
            SaveProduct(table, potato);

            var fruits = GetProductsFromCategory(table, "Fruits");
            foreach (var fruit in fruits)
            {
                Console.WriteLine("Fruit: {0}", fruit.Name);
            }

            RemoveProduct(table, "Fruits", "Mango");
        }

        static CloudTable SetupTable(string accountName, string key)
        {
            StorageCredentials creadentials = new StorageCredentials(accountName, key);

            CloudStorageAccount storageAccount = new CloudStorageAccount(creadentials, true);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            var tableName = "Products";

            CloudTable table = tableClient.GetTableReference(tableName);

            table.CreateIfNotExists();
            Console.WriteLine("Table Products created");

            return table;
        }

        static void SaveProduct(CloudTable table, Product product)
        {
            var saveOperation = TableOperation.InsertOrReplace(product);

            table.Execute(saveOperation);

            Console.WriteLine("Product {0} saved", product.Name);
            Console.ReadLine();
        }

        static void RemoveProduct(CloudTable table, string category, string name)
        {
            var product = new Product() {Category = category, Name = name};
            var removeOperation = TableOperation.Delete(product);
            table.Execute(removeOperation);
        }

        static IEnumerable<Product> GetProductsFromCategory(CloudTable table, string category)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, category);
            var query = new TableQuery<Product>().Where(filter);
            var result = table.ExecuteQuery<Product>(query);
            Console.WriteLine("{0} retrieved. Count: {1}", category, result.Count());
            return result;
        }

        static Product GetProduct(CloudTable table, string category, string name)
        {
            if (!String.IsNullOrEmpty(category) && !String.IsNullOrEmpty(name))
            {
                TableOperation retriveOperation = TableOperation.Retrieve<Product>(category,name);
                var result = table.Execute(retriveOperation);
                return result.Result as Product;
            }
            throw new Exception("Category and name of product must be provided");
        }

        static IEnumerable<Product> GetProducts(CloudTable table, double priceFrom, double priceTo)
        {
            var fromFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, priceFrom.ToString());
            var toFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThanOrEqual, priceTo.ToString());

            //Combine filters
            var filter = TableQuery.CombineFilters(fromFilter, TableOperators.And, toFilter);

            var query = new TableQuery<Product>().Where(filter);
            var result = table.ExecuteQuery<Product>(query);
            return result;
        }
    }

    
}

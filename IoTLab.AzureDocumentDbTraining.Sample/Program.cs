using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace IoTLab.AzureDocumentDbTraining.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string endpointUrl = "";
            string authorizationKey="";
            ConnectionPolicy connectionPolicy = new ConnectionPolicy() {ConnectionProtocol = Protocol.Tcp};
            var docClient = new DocumentClient(new Uri(endpointUrl), authorizationKey,
                    connectionPolicy);

            var databaseId = "DevicesDb";
            var collectionId = "Devices";

            var collectionSelfLink = String.Format("dbs/{0}/colls/{1}", databaseId, collectionId);
        }

        public static Database GetOrCreateDatabase(DocumentClient client, string id)
        {
            Database database = client.CreateDatabaseQuery().ToArray().FirstOrDefault(x => x.Id == id);
            if (database == null)
            {
                // Create the database.
                database = client.CreateDatabaseAsync(new Database { Id = id }).Result;
            }

            return database;
        }

        public static DocumentCollection GetOrCreateCollection(DocumentClient client, string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).ToArray().FirstOrDefault(x => x.Id == id);
            if (collection == null)
            {
                collection = client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id }).Result;
            }
            return collection;
        }

        public static async Task SaveAsync(DocumentClient client, string collectionSelfLink, Device device)
        {
            var document = getDocumentById(client, collectionSelfLink, device.Id);

            if (document != null)
            {
                await client.ReplaceDocumentAsync(document.SelfLink, device);
            }
            else
            {
                Document created = await client.CreateDocumentAsync(collectionSelfLink, device);
            }
        }

        public static Device FindDeviceById(DocumentClient client, string collectionSelfLink, string collectionId, string id)
        {
            var squery = String.Format("SELECT * FROM {0} item WHERE item.id=\'{1}\'", collectionId, id);
            var cquery = client.CreateDocumentQuery<Device>(collectionSelfLink, squery);
            var result = cquery.FirstOrDefault();
            return result;
        }

        public async Task RemoveAsync(DocumentClient client, string databaseId, string collectionId,string id)
        {
            var documentSelfLink = string.Format("dbs/{0}/colls/{1}/docs/{2}", databaseId, collectionId, id);
            await client.DeleteDocumentAsync(documentSelfLink);
        }

        private static Document getDocumentById(DocumentClient client, string collectionSelfLink, string id)
        {
            var sid = id.ToString();
            var cquery = client.CreateDocumentQuery(collectionSelfLink).Where(x => x.Id == sid);
            var documents = cquery.ToList();
            var document = documents.FirstOrDefault();
            return document;
        }
    }
}

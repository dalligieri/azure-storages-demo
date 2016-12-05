using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IoTLab.AzureRedisTraining.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private static string _connectionString = "";

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_connectionString);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public void AddDeviceToRegister(string deviceName)
        {
            var setName = "devices";
            var database = Connection.GetDatabase();
            if (!database.SetContains(setName, deviceName))
                database.SetAdd(setName, deviceName);
        }

        public IEnumerable<string> GetDevicesRegister()
        {
            var setName = "devices";
            var database = Connection.GetDatabase();
            return database.SetMembers(setName).ToStringArray();
        }

        public void SaveDeviceInfo(DeviceInfo device)
        {
            var database = Connection.GetDatabase();
            var data = JsonConvert.SerializeObject(device);
            var result = database.StringSet(device.Name, data);
            if (!result)
                throw new Exception("Not saved");
        }

        public DeviceInfo GetDeviceInfo(string name)
        {
            var database = Connection.GetDatabase();
            if (!database.KeyExists(name))
                return null;
            var data = database.StringGet(name);
            return JsonConvert.DeserializeObject<DeviceInfo>(data);
        }
    }
}

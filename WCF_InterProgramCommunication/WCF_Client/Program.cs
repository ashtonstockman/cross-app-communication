using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Client
{
    [ServiceContract]
    public interface IRedisCacher
    {
        [OperationContract]
        string ReadCacheValue(string value);

        [OperationContract]
        string WriteCacheValue(string key, string value);
    }

    class Program
    {
        public static string hostName = ConfigurationManager.AppSettings["hostName"].ToString();

        static void Main(string[] args)
        {
            var ep = $"net.tcp://{hostName}:9985/RedisCacher";

            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.PortSharingEnabled = false;
            tcpBinding.MaxConnections = 300;
            tcpBinding.ListenBacklog = 50;

            ChannelFactory<IRedisCacher> revFactory = new ChannelFactory<IRedisCacher>(tcpBinding, new EndpointAddress(ep));

            IRedisCacher revChannel = revFactory.CreateChannel();

            Console.WriteLine("Connected to: " + ep);
            while (true)
            {
                Console.WriteLine("Please select 1 to write values to redis and 2 to read values from redis.");
                switch (Console.ReadLine())
                {
                    case "1":
                        WriteToRedis(revChannel);
                        break;
                    case "2":
                        ReadFromRedis(revChannel);
                        break;
                    default:
                        Console.WriteLine("Please enter 1 or 2.  If done, close the window :D");
                        break;
                }
            }
        }

        private static void WriteToRedis(IRedisCacher revChannel)
        {
            Console.WriteLine("Please enter a key to write to local Redis cache...");
            var requestedKey = Console.ReadLine();

            Console.WriteLine($"Please enter a value to save for key: {requestedKey}.");
            var setVal = Console.ReadLine();

            Console.WriteLine("response: " + revChannel.WriteCacheValue(requestedKey, setVal));
        }

        private static void ReadFromRedis(IRedisCacher revChannel)
        {
            Console.WriteLine("Please enter a key to retrieve from local Redis cache...");
            var requestedKey = Console.ReadLine();

            Console.WriteLine("response: " + revChannel.ReadCacheValue(requestedKey));
        }
    }
}
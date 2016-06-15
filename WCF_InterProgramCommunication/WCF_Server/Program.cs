using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace WCF_Server
{
    [ServiceContract]
    public interface IRedisCacher
    {
        [OperationContract]
        string ReadCacheValue(string value);

        [OperationContract]
        string WriteCacheValue(string key, string value);
    }

    public class RedisCacher : IRedisCacher
    {
        public static string redisConnString = "192.168.99.100:32768";

        public string ReadCacheValue(string key)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnString);
            var db = redis.GetDatabase(0);
            var output = $"The requested key {key} has a value of {db.StringGet(key)}.";
            Console.WriteLine(output);
            return output;
        }

        public string WriteCacheValue(string key, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnString);
            var db = redis.GetDatabase(0);

            db.StringSet(key, value);

            var output = $"The key {key} has an updated value of {db.StringGet(value)}.";
            Console.WriteLine(output);
            return output;
        }
    }

    class Program
    {
        public static string hostName = ConfigurationManager.AppSettings["hostName"].ToString();

        static void Main(string[] args)
        {
            var uriString = "net.tcp://" + hostName + ":9985";
            Console.WriteLine("Opening connection on: " + uriString);

            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.PortSharingEnabled = false;
            tcpBinding.MaxConnections = 300;
            tcpBinding.ListenBacklog = 50;

            var uri = new Uri[] { new Uri("net.tcp://" + System.Net.Dns.GetHostName() + ":9985") };

            using (var host = new ServiceHost(typeof(RedisCacher), uri))
            {
                host.AddServiceEndpoint(typeof(IRedisCacher), tcpBinding, "RedisCacher");

                host.Open();

                Console.WriteLine("Server is available.  Press any key to exit.");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}
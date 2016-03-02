using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Server
{
    [ServiceContract]
    public interface IStringReverser
    {
        [OperationContract]
        string ReverseString(string value);
    }

    public class StringReverser : IStringReverser
    {
        public string ReverseString(string value)
        {
            char[] retVal = value.ToCharArray();
            int idx = 0;
            for (int i = value.Length - 1; i >= 0; i--)
                retVal[idx++] = value[i];

            string result = new string(retVal);
            Console.WriteLine(value + " -> " + result);
            return result;
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

            using (var host = new ServiceHost(typeof(StringReverser), uri))
            {
                host.AddServiceEndpoint(typeof(IStringReverser), tcpBinding, "TcpReverse");

                host.Open();

                Console.WriteLine("Server is available.  Press any key to exit.");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}
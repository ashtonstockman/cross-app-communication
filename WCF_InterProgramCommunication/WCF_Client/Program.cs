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
    public interface IStringReverser
    {
        [OperationContract]
        string ReverseString(string value);
    }

    class Program
    {
        public static string hostName = ConfigurationManager.AppSettings["hostName"].ToString();

        static void Main(string[] args)
        {
            var ep = $"net.tcp://{hostName}:9985/TcpReverse";

            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.PortSharingEnabled = false;
            tcpBinding.MaxConnections = 300;
            tcpBinding.ListenBacklog = 50;

            ChannelFactory<IStringReverser> revFactory = new ChannelFactory<IStringReverser>(tcpBinding, new EndpointAddress(ep));

            IStringReverser revChannel = revFactory.CreateChannel();

            Console.WriteLine("Connected to: " + ep);
            while (true)
            {
                string str = Console.ReadLine();
                Console.WriteLine("response: " + revChannel.ReverseString(str));
            }
        }
    }
}
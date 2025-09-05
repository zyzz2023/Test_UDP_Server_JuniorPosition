using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace Test_UDP_Server_JuniorPosition
{
    public class Program
    {
        private const int listenPort = 8888;
        
        static void Main(string[] args)
        {
            UDPListener listener = new UDPListener();
            
            listener.RecieveMetrics();
        }

        
    }
}

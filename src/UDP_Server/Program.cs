using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace Test_UDP_Server_JuniorPosition
{
    public class Program
    {
        static void Main(string[] args)
        {
            UDPServer listener = new UDPServer();
            listener.StartServer();

            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                listener.StopServer();
            }
        }
    }
}

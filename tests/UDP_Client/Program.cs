using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (UdpClient client = new UdpClient())
            {
                string message = "cpu_load:-3.3 ";
                byte[] data = Encoding.UTF8.GetBytes(message);

                client.Send(data, data.Length, "localhost", 8888);
                Console.WriteLine($"Отправлено: {message}");
            }
        }
    }
}

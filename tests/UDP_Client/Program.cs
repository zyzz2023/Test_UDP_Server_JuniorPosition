using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (UdpClient client = new UdpClient())
            {
                //for(int i = 0; i < 20; i++)
                //{
                //    Thread.Sleep(100);
                //    string message = $"metric_number:{i}";
                //    byte[] data = Encoding.UTF8.GetBytes(message);

                //    client.Send(data, data.Length, "localhost", 8888);
                //    Console.WriteLine($"Отправлено: {message}");
                //}
                string message = $"temp_cpu:4";
                byte[] data = Encoding.UTF8.GetBytes(message);

                client.Send(data, data.Length, "localhost", 8888);
                Console.WriteLine($"Отправлено: {message}");
            }
        }
    }
}

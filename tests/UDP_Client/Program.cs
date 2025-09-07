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
            Console.WriteLine("Вводите сообщения. Для выхода введите 'exit'");
            bool _isRunning = true;

            using (UdpClient client = new UdpClient())
            {
                while (_isRunning)
                {
                    Console.Write("> ");
                    string message = Console.ReadLine();

                    if (message == "exit")
                        break;

                    byte[] data = Encoding.UTF8.GetBytes(message);

                    client.Send(data, data.Length, "localhost", 8888);
                    Console.WriteLine($"Отправлено: {message}");
                }
                //for(int i = 0; i < 20; i++)
                //{
                //    Thread.Sleep(100);
                //    string message = $"metric_number:{i}";
                //    byte[] data = Encoding.UTF8.GetBytes(message);

                //    client.Send(data, data.Length, "localhost", 8888);
                //    Console.WriteLine($"Отправлено: {message}");
                //}
            }
            Console.WriteLine("Программа завершена.");
        }
        //static void Main(string[] args)
        //{
        //    Console.InputEncoding = Encoding.UTF8;
        //    Console.OutputEncoding = Encoding.UTF8;

        //    using (UdpClient client = new UdpClient())
        //    {
        //        Console.WriteLine("Вводите сообщения. Для выхода введите 'exit'");
        //        while (Console.ReadKey().Key != ConsoleKey.Escape)
        //        {
        //            Console.Write("> ");
        //            string message = Console.ReadLine();

        //            if (message?.ToLower() == "exit")
        //                break;

        //            Console.WriteLine($"Вы сказали: {message}");
        //        }
        //        //for(int i = 0; i < 20; i++)
        //        //{
        //        //    Thread.Sleep(100);
        //        //    string message = $"metric_number:{i}";
        //        //    byte[] data = Encoding.UTF8.GetBytes(message);

        //        //    client.Send(data, data.Length, "localhost", 8888);
        //        //    Console.WriteLine($"Отправлено: {message}");
        //        //}
        //        //string message = $"temp_cpu:4";
        //        //byte[] data = Encoding.UTF8.GetBytes(message);
        //        //
        //        //client.Send(data, data.Length, "localhost", 8888);
        //        //Console.WriteLine($"Отправлено: {message}");
        //    }
        //}
    }
}

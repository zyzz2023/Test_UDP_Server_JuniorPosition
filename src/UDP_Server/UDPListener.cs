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
    public class UDPListener
    {
        private const int listenPort = 8888;
        public void RecieveMetrics()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                Console.WriteLine("Ожидание метрики...");
                byte[] recievedData = listener.Receive(ref groupEP);

                string metric = Encoding.UTF8.GetString(recievedData);
                Console.WriteLine($"Получена метрика {metric}");

                if (TryParseMetric(metric, out string name, out double value, out string error))
                {
                    Console.WriteLine($" [METRIC] {name} = {value}");
                }
                else
                {
                    Console.WriteLine($"Ошибка формата: {metric}");
                    Console.WriteLine($"{error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private bool TryParseMetric(string message, out string name, out double value, out string error)
        {
            name = null;
            value = 0;
            error = null;

            var parts = message.Split(':');
            if (parts.Length != 2)
            {
                error = "Отсутствует разделитель ':'";
                return false;
            }

            name = parts[0].Trim();
            if (string.IsNullOrEmpty(name))
            {
                error = "Имя метрики не может быть пустым";
                return false;
            }

            string valueString = parts[1].Trim();
            if (string.IsNullOrEmpty(valueString))
            {
                error = "Значение метрики не может быть пустым";
                return false;
            }

            if (valueString.Contains(','))
            {
                error = "Запятая не допускается. Используйте точку как разделитель";
                return false;
            }

            if (double.TryParse(valueString, System.Globalization.NumberStyles.Any,
                CultureInfo.GetCultureInfo("en-US"), out value))
            {
                return true;
            }

            return false;
        }
    }
}

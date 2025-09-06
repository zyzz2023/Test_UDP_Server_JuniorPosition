using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Threading;

namespace Test_UDP_Server_JuniorPosition
{
    public class UDPServer
    {
        private const int _listenPort = 8888;
        private object _locker = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private UdpClient _listener;
        private Thread _receiverThread;
        private Thread _workingThread;

        private Queue<string> _queue = new Queue<string>();
        private Dictionary<string, double> _dictionary = new Dictionary<string, double>();

        public void StartServer()
        {
            Console.WriteLine("Сервер запущен.");
            Console.WriteLine("Для завершения нажмите Enter.");

            _receiverThread = new Thread(new ThreadStart(ReceiveMetrics));
            _workingThread = new Thread(new ThreadStart(WorkWithMetrics));

            _receiverThread.Start();
            _workingThread.Start();

        }

        public void StopServer()
        {
            _cts.Cancel();
            _listener.Close();

            if (_receiverThread != null && _receiverThread.IsAlive)
                _receiverThread.Join();

            if (_workingThread != null && _workingThread.IsAlive)
                _workingThread.Join();
        }
        public void ReceiveMetrics()
        {
            try
            {
                _listener = new UdpClient(_listenPort);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Не удалось открыть порт {_listenPort}: {ex.Message}");
                if (_receiverThread != null && _receiverThread.IsAlive)
                    _receiverThread.Join();

                if (_workingThread != null && _workingThread.IsAlive)
                    _workingThread.Join();
                return;
            }
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, _listenPort);

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    byte[] receivedData = _listener.Receive(ref groupEP);
                    if (receivedData.Length > 256)
                    {
                        continue;
                    }
                    string metric = Encoding.UTF8.GetString(receivedData);

                    if (TryParseMetric(metric, out string name, out double value, out string error))
                    {
                        lock (_locker)
                        {
                            _queue.Enqueue(metric);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка формата: {metric}");
                    }
                }
                catch (SocketException ex)
                {
                    if (_cts.Token.IsCancellationRequested) // Чтобы при завершении не выводилась ошибка прерывания Receive
                    {
                        Console.WriteLine("Поток приёма остановлен.");
                        break;
                    }
                    Console.WriteLine($"Сетевое исключение: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            _listener.Dispose(); 
        }
        public void WorkWithMetrics()
        {
            var checkQueueInterval = TimeSpan.FromMilliseconds(250);
            var printMetricsInterval = TimeSpan.FromMilliseconds(5000);

            var lastCheckQueueTime = DateTime.Now;
            var lastPrintMetricsTime = DateTime.Now;
            while (!_cts.Token.IsCancellationRequested)
            {
                var now = DateTime.Now;
                
                if (now - lastCheckQueueTime >= checkQueueInterval)
                {
                    CheckQueue();
                    lastCheckQueueTime = now;
                }

                if (now - lastPrintMetricsTime >= printMetricsInterval)
                {
                    PrintMetrics();
                    lastPrintMetricsTime = now;
                }
            }
        }
        
        private void CheckQueue()
        {
            
            if (_queue.Count > 0)
            {
                string metric = null;
                
                lock (_locker)
                {
                    metric = _queue.Dequeue();
                }
                TryParseMetric(metric, out string name, out double value, out string error);

                lock (_locker)
                {
                    if (_dictionary.ContainsKey(name))
                    {
                        _dictionary[name] = value;
                    }
                    else
                    {
                        _dictionary.Add(name, value);
                    }
                }
            }
        }
        private void PrintMetrics()
        {
            
            if (_dictionary.Count > 0)
            {
                lock (_locker)
                {
                    Console.Write($"[METRIC] ");
                    foreach (var name in _dictionary.Keys)
                    {
                        Console.Write($"{name} = {_dictionary[name]} | ");
                    }
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("[METRIC] Нет данных");
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

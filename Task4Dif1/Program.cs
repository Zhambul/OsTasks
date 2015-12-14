using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Task4Dif1
{
    class Program
    {
        private static ListenerExecutor _inputExecutor;
        private static ListenerExecutor _outputExecutor;
        private static readonly int[] Table;
        public static object EnqueObject;
        public static object ShowObject;
        static Program()
        {
            ShowObject = new object();
            EnqueObject = new object();
            _inputExecutor = new ListenerExecutor(3);
            _outputExecutor = new ListenerExecutor(1);
            _inputExecutor.NextExecutor = _outputExecutor;

            Table = new int[9];

            for (int i = 0; i < Table.Length; i++)
            {
                Table[i] = i + 1;
            }
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < Table.Length; i++)
            {
                Thread thread = new Thread(Tr);
                thread.Name = i.ToString();
                thread.Start(Table[i]);
            }
        }

        static void Tr(object number)
        {
            while (true)
            {
                _inputExecutor.Notify((int) number);
                Thread.Sleep(new Random().Next(100, 500));
            }
        }

        public static void Show()
        {
            Console.Clear();
            Console.WriteLine("Таблица");
            foreach (var i in Table)
            {
                Console.Write(i + " ");
            }

            Console.Write(Environment.NewLine);
            Console.Write(Environment.NewLine);

            Console.WriteLine("Очередь на ввод");
            
            foreach (var i in _inputExecutor.Numbers.ToList())
            {
                Console.Write(i + " ");
            }

            Console.Write(Environment.NewLine);
            Console.Write(Environment.NewLine);

            for (int i = 0; i < _inputExecutor.Listeners.Count; i++)
            {
                var index = i + 1;
                var listener = _inputExecutor.Listeners[i];
                Console.WriteLine("Значение в клавиатуре" + index + "  -  " + listener.CurrentVallue);
            }

            Console.Write(Environment.NewLine);

            Console.WriteLine("Очередь на вывод");
            foreach (var i in _outputExecutor.Numbers.ToList())
            {
                Console.Write(i + " ");
            }
            Console.Write(Environment.NewLine);
            Console.Write(Environment.NewLine);

            Console.WriteLine("Значение в принтере  -  " + _outputExecutor.Listeners[0].CurrentVallue);
        }
    }

    class ListenerExecutor
    {
        public List<Listener> Listeners;

        public ConcurrentQueue<int> Numbers;

        private bool _isNotifying;

        public ListenerExecutor NextExecutor { get; set; }
        public ListenerExecutor(int numberOfListeners)
        {
            Listeners = new List<Listener>();

            for (int i = 0; i < numberOfListeners; i++)
            {
                Listeners.Add(new Listener());
            }
            Numbers = new ConcurrentQueue<int>();
        }

        public void Notify(int number)
        {
            lock (Program.EnqueObject)
            {
                Numbers.Enqueue(number);
            }
            lock (Numbers)
            {
                if (!_isNotifying)
                {
                    while (Numbers.Count > 0)
                    {
                        _isNotifying = true;
                        foreach (var listener in Listeners)
                        {
                            if (!listener.IsWorking)
                            {
                                listener.IsWorking = true;
                                int numberFromQueue;
                                Numbers.TryDequeue(out numberFromQueue);
                                new Thread(delegate()
                                {
                                    listener.DoJob(numberFromQueue);

                                    if (NextExecutor != null)
                                    {
                                        NextExecutor.Notify(numberFromQueue);
                                    }

                                }).Start();
                                break;
                            }
                        }
                    }
                    _isNotifying = false;
                }
            }
        }

    }

    class Listener
    {
        public bool IsWorking { get; set; }
        public int CurrentVallue { get; set; }

        public void DoJob(int number)
        {
            CurrentVallue = number;
            Thread.Sleep(new Random().Next(2000,3000));
            
            lock (Program.ShowObject)
            {
                Program.Show();
            }

            CurrentVallue = 0;
            IsWorking = false;
        }
    }
}

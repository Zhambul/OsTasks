using System;
using System.Threading;

namespace Task5Dif1
{
    class Program
    {
        static object _lockObject = new object();
        static object _lockObject2 = new object();
        static object _lockObject3 = new object();

        static int[] _memory = new int[1000];
        static int[] _indexes = new int[10];

        static Random _random = new Random();
        static void Main(string[] args)
        {
            while (true)
            {
                int fileIndex = _random.Next(1, 10);

                if (_indexes[fileIndex] == 0)
                {
                    Thread thread = new Thread(CreateThread);
                    thread.Start(fileIndex);

                    Thread.Sleep(_random.Next(200, 500));
                }
            }

        }

        static void CreateThread(object index)
        {
            int fileIndex = Convert.ToInt32(index);
            int fileSize = _random.Next(20, 100);
            int durationTime = _random.Next(1000, 2000);

            for (int i = 1; i < _memory.Length; i++)
            {
                if (_memory[i] == 0 && IsCorrectAmountOfMemory(i, fileSize))
                {
                    FillMemory(fileIndex, fileSize, i);

                    ShowMemory();

                    Thread.Sleep(durationTime);

                    RemoveFile(fileIndex, fileSize, i);

                    ShowMemory();

                    Thread.Sleep(200);

                    break;
                }
            }
        }
        static void FillMemory(int fileIndex, int fileSize, int i)
        {
            lock (_lockObject2)
            {
                for (int j = 0; j < fileSize; j++)
                {
                    _memory[i + j] = fileIndex;
                }

                _indexes[fileIndex] = i;
            }
        }

        static bool IsCorrectAmountOfMemory(int i, int fileSize)
        {
            bool checkIfSizeCorrect = true;

            if (_memory.Length < i + fileSize)
            {
                return false;
            }

            for (int j = 0; j < fileSize; j++)
            {
                if (_memory[i + j] != 0)
                {
                    checkIfSizeCorrect = false;
                }
            }
            return checkIfSizeCorrect;
        }

        static void RemoveFile(int fileIndex, int fileSize, int i)
        {
            lock (_lockObject3)
            {
                for (int j = 0; j < fileSize; j++)
                {
                    _memory[i + j] = 0;
                }

                _indexes[fileIndex] = 0;
            }
        }
        static void ShowMemory()
        {
            lock (_lockObject)
            {
                Console.Clear();

                for (int i = 1; i < _indexes.Length; i++)
                {
                    Console.Write(_indexes[i] + "  ");
                }

                Console.Write(Environment.NewLine);

                for (int i = 1; i < _memory.Length; i++)
                {
                    Console.Write(_memory[i]);
                }
            }
        }
    }
}

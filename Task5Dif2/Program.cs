using System;
using System.Threading;

namespace Task5Dif2
{
    class Program
    {
        static readonly object LockObject = new object();
        static readonly object LockObject2 = new object();
        static readonly object LockObject3 = new object();

        static readonly int[] Memory = new int[1000];
        static readonly int[] Indexes = new int[10];

        static readonly Random Random = new Random();

        static readonly int CellSize = 100;
        static void Main(string[] args)
        {

            while (true) { 
            int fileIndex = Random.Next(1, 10);

            if (Indexes[fileIndex] == 0)
            {
                Thread thread = new Thread(CreateThread);
                thread.Start(fileIndex);

                Thread.Sleep(Random.Next(200, 500));
            }
            }
        }

        static void CreateThread(object index)
        {
            int fileIndex = Convert.ToInt32(index);
            int fileSize = Random.Next(20, 100);
            int durationTime = Random.Next(1000, 2000);

            for (int i = 1; i < Memory.Length; i++)
            {
                if (Memory[i] == 0 && IsCorrectAmountOfMemory(i, fileSize))
                {
                    FillMemory(fileIndex, fileSize, i, true);

                    ShowMemory();

                    Thread.Sleep(durationTime);

                    RemoveFile(fileIndex, fileSize, i);

                    ShowMemory();

                    Thread.Sleep(200);

                    break;
                }
            }
        }
        static void FillMemory(int fileIndex, int fileSize, int currentIndex,bool writeToTable)
        {
            lock (LockObject2)
            {
                int fillSize = fileSize > CellSize ? CellSize : fileSize;

                for (int j = 0; j < fillSize; j++)
                {
                    Memory[currentIndex + j] = fileIndex;
                }

                if (writeToTable)
                {
                    Indexes[fileIndex] = currentIndex;
                }

                if (fileSize > CellSize)
                {
                    int indexOfNextCell = GetNextCellIndex();

                    FillMemory(fileIndex, fileSize - CellSize, indexOfNextCell, false);
                }
            }
        }

        private static int GetNextCellIndex()
        {
            while (true)
            {
                int randomIndex = Random.Next(1, 10);

                if (Indexes[randomIndex] == 0)
                {
                    if (Memory[randomIndex*CellSize] == 0)
                    {
                        return randomIndex;
                    }
                }
            }
        }

        static void RemoveFile(int fileIndex, int fileSize, int i)
        {
            lock (LockObject3)
            {

                for (int j = 0; j < fileSize; j++)
                {
                    Memory[i + j] = 0;
                }

                Indexes[fileIndex] = 0;
            }
        }
        static bool IsCorrectAmountOfMemory(int currentIndex, int fileSize)
        {
            bool checkIfSizeCorrect = true;

            if (Memory.Length < currentIndex + fileSize)
            {
                return false;
            }

            int checkSize = fileSize > CellSize ? CellSize : fileSize;
            
            for (int j = 0; j < checkSize; j++)
            {
                if (Memory[currentIndex + j] != 0)
                {
                    checkIfSizeCorrect = false;
                }
            }

            return checkIfSizeCorrect;
        }

        static void ShowMemory()
        {
            lock (LockObject)
            {
                Console.Clear();

                for (int i = 1; i < Indexes.Length; i++)
                {
                    Console.Write(Indexes[i]);
                    Console.Write(Indexes[i].ToString().Length == 1 ? "    "
                                : Indexes[i].ToString().Length == 2 ? "   "
                                : "  ");
                }

                Console.Write(Environment.NewLine);

                for (int i = 1; i < Memory.Length; i++)
                {
                    Console.Write(Memory[i]);

                    if (i % CellSize == 0)
                    {
                        Console.WriteLine(Environment.NewLine);
                    }
                }
            }
        }
    }
}

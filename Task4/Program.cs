using System;
using System.Threading;

namespace Task4
{
    class Program
    {
        public static int[] Memory = new int[10];
        public static int[] Mem = new int[10];
        static Random n = new Random();

        public static void ShowMemory(int[] mem)
        {
            Console.Clear();
            for (int i = 0; i <= 9; i++)
            {
                Console.Write(Memory[i]);
            }
            Console.WriteLine();
            for (int i = 0; i <= 9; i++)
            {
                Console.Write(mem[i]);
            }
        }
        public static void Read(int block, int index, int[] mem)
        {
            bool flag = false;
            for (int i = 1; i < Memory.Length; i++)
            {
                if (Memory[i] == 0)
                {
                    Memory[i] = block;
                    mem[index] = i;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (!flag && Memory[i] == block)
                    {
                        for (int j = 1; j <= 9; j++)
                        {
                            mem[j] = 0;
                            mem[index] = index;
                            flag = true;
                            break;
                        }
                    }
                }
            }
        }

        public static void Tr(object ob)
        {
            int x = Convert.ToInt32(ob);
            int[] mem = new int[10];
            while (true)
            {
                Thread.Sleep(n.Next(500, 2000));
                int index = n.Next(0, 9);
                if (mem[index] == 0)
                    Read(x, index, mem);
                ShowMemory(mem);

            }
        }

        static void Main(string[] args)
        {
            Memory = new int[10];
            new Thread(Tr).Start(1);
            new Thread(Tr).Start(2);
            new Thread(Tr).Start(3);
            Console.ReadKey();
        }
    }
}

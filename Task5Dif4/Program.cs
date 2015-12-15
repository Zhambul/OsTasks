using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task5Dif4
{
    class Program
    {
        static readonly Cell MainCell;

        private const int InnerCellSize = 128;

        static Program()
        {
            MainCell = new Cell(13);

            for (int i = 0; i < 9; i++)
            {
                MainCell.InnerCells[i] = new Cell(1);
            }

            for (int i = 9; i < 13; i++)
            {
                MainCell.InnerCells[i] = new Cell(InnerCellSize);
            }

            for (int i = 11; i < 13; i++)
            {
                for (int j = 0; j < InnerCellSize; j++)
                {
                    MainCell.InnerCells[i].InnerCells[j] = new Cell(InnerCellSize);
                }
            }
            
            for (int j = 0; j < InnerCellSize; j++)
            {
                for (int k = 0; k < InnerCellSize; k++)
                {
                    MainCell.InnerCells[12].InnerCells[j].InnerCells[k] = new Cell(InnerCellSize);
                }
            }
            
        }

        static void Main(string[] args)
        {

        }

        static void Tr()
        {
            int fileSize = new Random().Next(1, 400);


        }

        static void FillMemory(int fileSize)
        {
             
        }

        static void ShowMemeory(Cell[] cells, bool clearConsole)
        {
            if (clearConsole)
            {
                Console.Clear();
            }

            foreach (var cell in MainCell.InnerCells)
            {
                if (cell.Value == 0)
                {
                    break;
                }

                Console.Write(cell.Value + " ");

                if(cell.InnerCells != null)
                {
                    ShowMemeory(cell.InnerCells, false);
                }
            }

        }
    }

    class Cell
    {
        public int Value { get; set; }
        public List<Cell> InnerCells { get; set; }
        public int Size { get; set; }
        public Cell(int size)
        {
            Size = size;
        }
    }

    class MemoryFiller
    {
        
    }
}

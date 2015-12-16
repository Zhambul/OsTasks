using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task5Dif4
{
    class Program
    {
        static readonly List<Cell> MainCells;

        private const int InnerCellSize = 128;

        private static int Count = 0;

        private static object MemoryLockObject = new object();
        private static object ShowLockObject = new object();

         static Program()
        {
            MainCells = InitListOfCells(13);

            for (int i = 10; i < 13; i++)
            {
                MainCells[i].InnerCells = InitListOfCells(InnerCellSize);
            }

           
            for (int i = 11; i < 13; i++)
            {
                for (int j = 0; j < InnerCellSize; j++)
                {
                    MainCells[i].InnerCells[j].InnerCells = InitListOfCells(InnerCellSize);
                }
            }
            
            for (int j = 0; j < InnerCellSize; j++)
            {
                for (int k = 0; k < InnerCellSize; k++)
                {
                    MainCells[12].InnerCells[j].InnerCells[k].InnerCells = InitListOfCells(InnerCellSize);
                }
            }

            SetId(MainCells,0);
        }

        private static int SetId(List<Cell> cells, int id)
        {
            foreach (Cell cell in cells)
            {
                if (cell.InnerCells != null)
                {
                    cell.Id = Cell.InvalidId;
                    id = SetId(cell.InnerCells,id);
                }
                else
                {
                    cell.Id = id;
                    id++;
                }
            }
            return id;
        }

        static List<Cell> InitListOfCells(int size)
        {
            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < size; i++)
            {
                cells.Add(new Cell());
            }

            return cells;
        }   

        static void Main(string[] args)
        {
            for (int i = 0; i < 4; i++)
            {
                Thread thread = new Thread(Tr);
                thread.Name = i.ToString();
                thread.Start();
                Thread.Sleep(new Random().Next(100,500));
            }
            Console.ReadLine();
//            Tr();
        }

        static void Tr()
        {
            int fileSize = 5;
            int fileValue = new Random().Next(1, 10);
            var file = new File(fileValue,fileSize);

//            lock (MemoryLockObject)
//            {
                Debug.WriteLine("filling memory on thread " + Thread.CurrentThread.Name);
                FillMemory(file);
//            }
            lock (ShowLockObject)
            {
                Debug.WriteLine("showing memory on thread " + Thread.CurrentThread.Name);
                ShowMemory(MainCells, true);
            }
//            Thread.Sleep(1000);
////            lock (MemoryLockObject)
////            {
//            Debug.WriteLine("deleting memory on thread " + Thread.CurrentThread.Name);
//                DeleteFile(MainCells, file);
////            }
//            lock (ShowLockObject)
//            {
//                ShowMemory(MainCells, true);
//            }
//            Thread.Sleep(1000);
//            Console.ReadLine();
        }

        private static void DeleteFile(List<Cell> cells, File file)
        {
            foreach (Cell cell in cells)
            {
                if (file.Size == 0)
                {
                    break;
                }
                if (cell.InnerCells != null)
                {
                    DeleteFile(cell.InnerCells, file);
                }
                else
                {
                    if ((file.FirstCellId-1) == cell.Id)
                    {
                        cell.Value = 0;
                        file.FirstCellId++;
                        file.Size--;
                    }
                }
            };
        }

        static void FillMemory(File file)
        {
            int cellId = GetAvailableCellId(MainCells, file,0);
            FillCell(MainCells, file, cellId);   
        }

        // на объем памяти файла дать id клетки с которой вместится файл
        private static int GetAvailableCellId(List<Cell> cells, File file,int checkingCellId)
        {
            foreach (var cell in cells)
            {
                if (cell.InnerCells != null)
                {
                    checkingCellId = GetAvailableCellId(cell.InnerCells, file, checkingCellId);
                }
                else
                {
                    if (CheckNextCells(checkingCellId, file))
                    {
                        return checkingCellId;
                    }
                    checkingCellId++;
                }
            }
            return checkingCellId;
        }

        private static bool CheckNextCells(int checkingCellId, File file)
        {
            for (int i = checkingCellId; i < file.Size; i++)
            {
                if (GetCellById(MainCells,checkingCellId).Value != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static Cell GetCellById(List<Cell> cells,int cellId)
        {
            foreach (var cell in cells)
            {
                if (cell.InnerCells != null)
                {
                    Cell resultCell = GetCellById(cell.InnerCells,cellId);
                    if (resultCell != null)
                    {
                        return resultCell;
                    }
                }
                else
                {
                    if (cell.Id == cellId)
                    {
                        return cell;
                    }
                }
            }
            return null;
        }
        static int FillCell(List<Cell> cells, File file, int cellId)
        {
            foreach (Cell cell in cells)
            {
                if (file.IsDone)
                {
                    break;
                }
                if (cell.InnerCells != null)
                {
                   cellId = FillCell(cell.InnerCells, file, cellId);
                }
                else
                {
                    if (cell.Id == cellId)
                    {
                        cell.Value = file.GetNextValue();

                        if (file.FirstCellId == 0)
                        {
                            file.FirstCellId = cell.Id;
                        }
                        cellId++;
                    }
                }
            }
            return cellId;
        }
       
        static void ShowMemory(List<Cell> cells , bool clearConsole)
        {
            if (clearConsole)
            {
                Console.Clear();
            }

            foreach (var cell in cells)
            {
                if (cell.Value != Cell.InvalidValue && cell.Value != Cell.NoValue)
                {
//                    Count++;
                    Console.Write(cell.Value + " ");
                }

                if(cell.InnerCells != null)
                {
                    ShowMemory(cell.InnerCells, false);
                }
            }
        }
    }

    class Cell
    {
        public static readonly int InvalidValue = -1;
        public static readonly int InvalidId = -1;
        public static readonly int NoValue = 0;

        private int _value;

        public int Id { get; set; }
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == InvalidValue)
                {
                    throw new Exception("file has inner cells");
                }
                _value = value;
            }
        }

        private List<Cell> _innerCells; 
        public List<Cell> InnerCells {
            get
            {
                return _innerCells;
            }
            set
            {
                _innerCells = value;
                Value = InvalidValue;
            } 
        }
    }

    class File
    {
        public static readonly int InvalidValue = -1;
        public int Value { get; set; }
        public int Size { get; set; }
        public int UnreadValueIndex { get; set; }
        public bool IsDone { get; set; }
        public int FirstCellId { get; set; }
        public File(int value, int size)
        {
            Value = value;
            Size = size;
            UnreadValueIndex = 1;
            IsDone = false;
        }

        public int GetNextValue()
        {
            if (UnreadValueIndex == Size)
            {
                IsDone = true;
            }
            UnreadValueIndex++;
            return Value;
        }
    }
}

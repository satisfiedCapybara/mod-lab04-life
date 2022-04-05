using System;
using System.Collections.Generic;
using System.Linq;
using StatusFileController;
using PropertyFileController;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ClassificationTK;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        public void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    class Program
    {
        static Board board;
        static private void Reset(LifeProperty aLifeProperty)
        {
            board = new Board(
                aLifeProperty.BoardWidth,
                aLifeProperty.BoardHeight,
                aLifeProperty.BoardCellSize,
                aLifeProperty.LifeDensity);
        }

        public static int AliveCellsNumber()
        {
            int AliveCellsNum = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    if (board.Cells[col, row].IsAlive)
                    {
                        AliveCellsNum ++;
                    }
                }
            }

            return AliveCellsNum ++;
        }

        static int StabilityPhase(ref Board theBoard)
        {
            Board aStartBoard = new Board(board.Width, board.Height, 1);

            Cell[,] aFirstCellState = new Cell[theBoard.Width,theBoard.Height];
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    aFirstCellState[row, col] = new Cell();
                    aFirstCellState[row, col].IsAlive = board.Cells[row,col].IsAlive;
                }
            }
            aStartBoard.Cells = aFirstCellState;
            aStartBoard.ConnectNeighbors();

            int aCellsNumber = 0;
            int aStep = 0;
            while(true)
            {
                Cell[,] aPreviousBorder = new Cell[theBoard.Width,theBoard.Height];

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)   
                    {
                        aPreviousBorder[row, col] = new Cell();
                        aPreviousBorder[row, col].IsAlive = board.Cells[row,col].IsAlive;
                    }
                }
                board.Advance();

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)   
                    {
                        if (aPreviousBorder[row,col].IsAlive == board.Cells[row, col].IsAlive)
                        {
                            aCellsNumber++;
                        }
                    }
                }

                aStep ++;
                if(aCellsNumber == board.Columns * board.Rows || aStep > 1000)
                {
                    break;
                }
                aCellsNumber = 0;
            }
            
            board = aStartBoard;

            return aStep;
        }

        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }

        public static void Main(string[] args)
        {
            LifeProperty aLifeProperty;
            JSONController.DeserializeFromJSON(out aLifeProperty, "LifeProperty.json");
            Reset(aLifeProperty);

            string[] aShapeNames = {"BARGE",
                                    "BEEHIVE",
                                    "BLOCK",
                                    "BOAT",
                                    "BOX",
                                    "LOAF",
                                    "POND",
                                    "SHIP"};            

            while(true)
            {
                TextController.SaveLifeStatus(board.Cells, "LifeStatus.txt");

                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
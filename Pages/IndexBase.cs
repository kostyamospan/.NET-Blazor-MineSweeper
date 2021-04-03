using System;
using System.Collections.Generic;
using System.Linq;
using BlazorMinesweeper.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorMinesweeper.Pages
{
    public class IndexBase : ComponentBase
    {
        protected int GridSize;

        protected int FlagsLeft { get; private set; }

        protected bool IsGameWin { get; private set; }
        protected bool IsGameLost { get; private set; }


        protected CellModel[][] GridValues { get; private set; }

        private HashSet<(int posX, int posY)> _flagsSet;

        private int _bombsCount;

        public IndexBase()
        {
            StartNewGame();
        }

        private void InitProperties()
        {
            GridSize = 10;
            _bombsCount = 10;
            FlagsLeft = _bombsCount;
            _flagsSet = new HashSet<(int posX, int posY)>();
            IsGameLost = false;
            IsGameWin = false;
            GridValues = null;
        }


        private void StartNewGame()
        {
            InitProperties();

            InitializeGridValuesList();

            GenerateGridBombs();

            CellCountBombsAround();
        }

        protected void OnPlayBtnPressed(MouseEventArgs obj)
        {
            StartNewGame();
        }

        protected void OnCellLeftBtnClicked(MouseEventArgs ev, (int x, int y) btnPosition)
        {
            var (x, y) = btnPosition;

            var cell = GridValues[x][y];
            // if (cell.Value == -1)
            //     IsGameLost = true;

            if (cell.IsOpened)
                OpenCellsAround(x, y);
            
            OpenCell(btnPosition);
        }

        private void OpenCellsAround(int x, int y)
        {
            int flagsAroundCount = 0;


            for (int c = x - 1; c <= x + 1; c++)
            {
                for (int d = y - 1; d <= y + 1; d++)
                {
                    if (GridValues[c][d].IsChecked) flagsAroundCount++;
                }
            }

            Console.WriteLine(flagsAroundCount);
            if (flagsAroundCount >= GridValues[x][y].Value)
            {
                for (int c = x - 1; c <= x + 1; c++)
                {
                    for (int d = y - 1; d <= y + 1; d++)
                    {
                        OpenCell((c, d));
                    }
                }
            }
        }


        protected void OnCellRightBtnClicked(MouseEventArgs ev, (int iCur, int jCur) btnPosition)
        {
            
            var (x, y) = btnPosition;

            var cell = GridValues[x][y];

            if (cell.IsOpened) return;

            if (cell.IsChecked)
            {
                _flagsSet.Remove((x, y));
                FlagsLeft++;
            }
            else
            {
                Console.WriteLine(FlagsLeft);
                if(FlagsLeft <= 0) return;
                
                _flagsSet.Add((x, y));
                FlagsLeft--;

                if (IsFlagsSetRight())
                {
                    IsGameWin = true;
                }
            }

            cell.IsChecked = !cell.IsChecked;

            StateHasChanged();
        }

        private bool IsFlagsSetRight()
        {
            int rightFlagsCount = 0;
            for (int i = 0; i < GridValues.Length; i++)
            {
                for (int j = 0; j < GridValues[i].Length; j++)
                {
                    if (GridValues[i][j].Value == -1 && _flagsSet.Contains((i, j)))
                        rightFlagsCount++;
                }   
            }

            return rightFlagsCount == _bombsCount;
        }
        
        private void OpenCell((int x, int y) btnPosition)
        {
            var (i, j) = btnPosition;

            var cell = GridValues[i][j];

            if (cell.IsOpened || cell.IsChecked)
                return;

            if (cell.Value == -1)
                IsGameLost = true;

            if (cell.Value == 0)
                OpenSurroundingCellsRecursive(btnPosition);


            cell.IsOpened = true;
        }

        private void OpenSurroundingCellsRecursive((int x, int y) btnPosition)
        {
            var (x, y) = btnPosition;

            if (!IsBounds(x, y, GridValues) || GridValues[x][y].IsOpened)
                return;

            var cell = GridValues[x][y];

            if (cell.Value != -1 && cell.Value != 0)
            {
                cell.IsOpened = true;
                return;
            }

            cell.IsOpened = true;


            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y) continue;

                    OpenSurroundingCellsRecursive((i, j));
                }
            }
        }

        private static bool IsBounds<T>(int i, int j, T[][] collection)
            => (i >= 0 && i < collection.Length) && (j >= 0 && j < collection[i].Length);

        private static int GenerateValue(int min = 0, int max = 1) => new Random().Next(min, max);

        private void InitializeGridValuesList()
        {
            GridValues = new CellModel[GridSize][];

            for (int i = 0; i < GridValues.Length; i++)
            {
                GridValues[i] = new CellModel[GridSize];

                for (int j = 0; j < GridValues[i].Length; j++)
                    GridValues[i][j] = new CellModel();
            }
        }

        private void GenerateGridBombs()
        {
            for (int i = 0; i < GridSize; i++)
            {
                int xCoord = 0, yCoord = 0;

                do
                {
                    xCoord = GenerateValue(max: GridSize);
                    yCoord = GenerateValue(max: GridSize);
                } while (GridValues[xCoord][yCoord].Value == -1);

                GridValues[xCoord][yCoord].Value = -1;
            }
        }

        private void CellCountBombsAround()
        {
            for (int i = 0; i < GridValues.Length; i++)
            {
                for (int j = 0; j < GridValues[i].Length; j++)
                {
                    if (GridValues[i][j].Value == -1) continue;

                    int bombsAround = 0;

                    for (int c = i - 1; c <= i + 1; c++)
                    {
                        for (int d = j - 1; d <= j + 1; d++)
                        {
                            if (IsBounds(c, d, GridValues))
                            {
                                if (GridValues[c][d].Value == -1)
                                    bombsAround++;
                            }
                        }
                    }

                    GridValues[i][j].Value = bombsAround;
                }
            }
        }
    }
    
}
using System;
using BlazorMinesweeper.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorMinesweeper.Pages
{
    public class IndexBase : ComponentBase
    {
        protected int _gridSize = 10;

        private int _bombsCount = 10;

        protected int FlagsLeft { get; private set; } = 10;

        protected bool IsGameOver { get; private set; } = false;

        protected CellModel[][] GridValues { get; private set; }

        public IndexBase()
        {
            InitializeGridValuesList();

            GenerateGridBombs();

            CellCountBombsAround();
        }

        protected override void OnInitialized()
        {
            // foreach (var item in _gridValues)
            // {
            //     foreach (var i in item)
            //         Console.Write("{0,-3}", i.Value);
            //
            //     Console.WriteLine();
            // }
        }

        protected void OnCellLeftBtnClicked(MouseEventArgs ev, (int x, int y) btnPosition)
        {
            OpenCell(btnPosition);

            StateHasChanged();
        }

        protected void OnCellRightBtnClicked(MouseEventArgs ev, (int iCur, int jCur) btnPosition)
        {
            var (x, y) = btnPosition;

            var cell = GridValues[x][y];

            if (cell.IsOpened) return;

            if (cell.IsChecked)
                FlagsLeft++;
            else
                FlagsLeft--;

            cell.IsChecked = !cell.IsChecked;

            StateHasChanged();
        }

        private void OpenCell((int x, int y) btnPosition)
        {
            var (i, j) = btnPosition;

            var cell = GridValues[i][j];

            if (cell.IsOpened || cell.IsChecked)
                return;

            if (cell.Value == -1)
                IsGameOver = true;

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
            GridValues = new CellModel[_gridSize][];

            for (int i = 0; i < GridValues.Length; i++)
            {
                GridValues[i] = new CellModel[_gridSize];

                for (int j = 0; j < GridValues[i].Length; j++)
                    GridValues[i][j] = new CellModel();
            }
        }

        private void GenerateGridBombs()
        {
            for (int i = 0; i < _gridSize; i++)
            {
                int xCoord = 0, yCoord = 0;

                do
                {
                    xCoord = GenerateValue(max: _gridSize);
                    yCoord = GenerateValue(max: _gridSize);
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
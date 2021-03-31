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

        protected int _flagsLeft = 10;

        protected bool isGameOver = false;

        protected CellModel[][] _gridValues;

        public IndexBase()
        {
            InitializeGridValuesList();

            GenerateGridBombs();

            CellCountBombsAround();
        }

        protected override void OnInitialized()
        {
            foreach (var item in _gridValues)
            {
                foreach (var i in item)
                    Console.Write("{0,-3}", i.Value);

                Console.WriteLine();
            }
        }

        protected void OnCellLeftBtnClicked(MouseEventArgs ev, (int x, int y) btnPosition)
        {
            OpenCell(btnPosition);

            StateHasChanged();
        }

        private void OpenCell((int x, int y) btnPosition)
        {
            var (i, j) = btnPosition;

            var cell = _gridValues[i][j];

            if (cell.IsOpened || cell.IsChecked)
                return;

            if (cell.Value == -1)
                isGameOver = true;

            if (cell.Value == 0)
                OpenSurroundingCellsRecursive(btnPosition);


            cell.IsOpened = true;
        }

        private void OpenSurroundingCellsRecursive((int x, int y) btnPosition)
        {
            var (x, y) = btnPosition;

            if (!IsBounds(x, y, _gridValues) || _gridValues[x][y].IsOpened)
                return;

            var cell = _gridValues[x][y];

            if (cell.Value != -1 && cell.Value != 0)
            {
                cell.IsOpened = true;
                return;
            }

            cell.IsOpened = true;

            OpenSurroundingCellsRecursive((x - 1, y));
            OpenSurroundingCellsRecursive((x + 1, y));

            OpenSurroundingCellsRecursive((x, y - 1));
            // ReSharper disable once TailRecursiveCall
            OpenSurroundingCellsRecursive((x, y + 1));
        }

        private static bool IsBounds<T>(int i, int j, T[][] collection)
            => (i >= 0 && i < collection.Length) && (j >= 0 && j < collection[i].Length);

        private static int GenerateValue(int min = 0, int max = 1) => new Random().Next(min, max);

        private void InitializeGridValuesList()
        {
            _gridValues = new CellModel[_gridSize][];

            for (int i = 0; i < _gridValues.Length; i++)
            {
                _gridValues[i] = new CellModel[_gridSize];

                for (int j = 0; j < _gridValues[i].Length; j++)
                    _gridValues[i][j] = new CellModel();
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
                } while (_gridValues[xCoord][yCoord].Value == -1);

                _gridValues[xCoord][yCoord].Value = -1;
            }
        }

        private void CellCountBombsAround()
        {
            for (int i = 0; i < _gridValues.Length; i++)
            {
                for (int j = 0; j < _gridValues[i].Length; j++)
                {
                    if (_gridValues[i][j].Value == -1) continue;

                    int bombsAround = 0;

                    for (int c = i - 1; c <= i + 1; c++)
                    {
                        for (int d = j - 1; d <= j + 1; d++)
                        {
                            if (IsBounds(c, d, _gridValues))
                            {
                                if (_gridValues[c][d].Value == -1)
                                    bombsAround++;
                            }
                        }
                    }

                    _gridValues[i][j].Value = bombsAround;
                }
            }
        }
    }
}
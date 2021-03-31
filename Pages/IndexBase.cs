using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorMinesweeper.Pages
{
    public class IndexBase : ComponentBase
    {
        protected int _gridSize = 10;

        private int _bombsCount = 10;

        protected int _flagsLeft = 10;

        private int[][] _gridValues;

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
                    Console.Write("{0,-3}", i);

                Console.WriteLine();
            }
        }

        protected void OnCellLeftBtnClicked(MouseEventArgs ev, (int x, int y) btnPosition)
        {
            Console.WriteLine($"X:{btnPosition.x}, Y:{btnPosition.y}");
        }

        private static bool IsBounds<T>(int i, int j, T[][] collection)
            => (i >= 0 && i < collection.Length) && (j >= 0 && j < collection[i].Length);

        private static int GenerateValue(int min = 0, int max = 1) => new Random().Next(min, max);

        private void InitializeGridValuesList()
        {
            _gridValues = new int[_gridSize][];

            for (int i = 0; i < _gridValues.Length; i++)
                _gridValues[i] = new int[_gridSize];
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
                } while (_gridValues[xCoord][yCoord] == -1);

                _gridValues[xCoord][yCoord] = -1;
            }
        }

        private void CellCountBombsAround()
        {
            for (int i = 0; i < _gridValues.Length; i++)
            {
                for (int j = 0; j < _gridValues[i].Length; j++)
                {
                    if(_gridValues[i][j] == -1) continue;
                    
                    int bombsAround = 0;

                    for (int c = i - 1; c <= i + 1; c++)
                    {
                        for (int d = j - 1; d <= j + 1; d++)
                        {
                            if (IsBounds(c, d, _gridValues))
                            {
                                if (_gridValues[c][d] == -1)
                                    bombsAround++;
                            }
                        }
                    }

                    _gridValues[i][j] = bombsAround;
                }
            }
        }
    }
}
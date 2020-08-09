using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class GameEngine
    {
        public uint CurrentGeneration { get; private set; } = default;
        private bool[,] field;
        private readonly int rows;
        private readonly int columns;

        public GameEngine(int _rows, int _columns, int _density)
        {
            this.rows = _rows;
            this.columns = _columns;

            field = new bool[_columns, _rows];
            Random random = new Random();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next(_density) == 0;
                }
            }
        }

        public bool[,] GetCurrentGeneration()
        {
            var result = new bool[columns, rows];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    result[x, y] = field[x, y];
                }
            }

            return result;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = default;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var column = (x + i + columns) % columns;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = column == x && row == y;
                    var hasLife = field[column, row];

                    if (hasLife && !isSelfChecking) count++;
                }
            }
            return count;
        }

        public void NextGeneration()
        {
            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];
                }
            }

            field = newField;
            CurrentGeneration++;
        }

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < columns && y < rows;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if (ValidateCellPosition(x, y)) field[x, y] = state;
        }

        public void AddCell(int x, int y)
        {
            UpdateCell(x, y, state: true);
        }

        public void RemoveCell(int x, int y)
        {
            UpdateCell(x, y, state: false);
        }
    }
}

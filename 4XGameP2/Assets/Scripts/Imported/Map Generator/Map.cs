// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE_CODE or copy
// at http://opensource.org/licenses/MIT)

namespace ImportedGenerator
{
    public class Map
    {
        private readonly int[,] map;

        public int this[int row, int col]
        {
            get => map[row, col];
            set => map[row, col] = value;
        }

        public int Rows => map.GetLength(0);
        public int Cols => map.GetLength(1);

        public Map(int rows, int cols)
        {
            map = new int[rows, cols];
        }
    }
}
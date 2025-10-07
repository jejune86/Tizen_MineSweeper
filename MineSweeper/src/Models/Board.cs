using System;

namespace MineSweeper.Models
{
    public class Board
    {
        public int rows = 10;
        public int cols = 10;
        public int mineCount = 15;

        public Cell[,] Cells { get; private set; }

        public Board()
        {
            Cells = new Cell[rows, cols];
        }

        public void Initialize(int firstRow, int firstCol)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Cells[r, c] = new Cell();
                }
            }
            
            Random rnd = new Random();
            int placedMines = 0;

            while (placedMines < mineCount)
            {
                int r = rnd.Next(rows);
                int c = rnd.Next(cols);
                if ((r == firstRow && c == firstCol) || Cells[r, c].num == -1) continue;
                Cells[r, c].num = -1;
                placedMines++;
            }

            // 주변 지뢰 수 계산
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (Cells[r, c].num == -1) continue;
                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = r + dr;
                            int nc = c + dc;
                            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                                if (Cells[nr, nc].num  == -1) count++;
                        }
                    Cells[r, c].num = count;
                }
        }
    }
}

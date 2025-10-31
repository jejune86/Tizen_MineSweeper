using System;

namespace MineSweeper.Models
{
    public class Cell
    {
        public int value;

        public bool isRevealed;

        public bool isFlagged;

        public int row;

        public int col;


        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
            value = 0;
            isRevealed = false;
            isFlagged = false;
        }

    }
}

using System;

namespace MineSweeper.Models
{
    public class Cell
    {
        public int num;

        public bool isRevealed;

        public bool isFlagged;


        public Cell()
        {
            this.num = 0;
            isRevealed = false;
            isFlagged = false;
        }

    }
}

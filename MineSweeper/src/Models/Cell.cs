using System;

namespace MineSweeper.Models
{
    public class Cell
    {
        public int value;

        public bool isRevealed;

        public bool isFlagged;


        public Cell()
        {
            value = 0;
            isRevealed = false;
            isFlagged = false;
        }

    }
}

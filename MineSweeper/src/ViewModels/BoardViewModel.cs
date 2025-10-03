using System;
using Tizen.NUI.Components;
using MineSweeper.Models;

namespace MineSweeper.ViewModels
{
    public class BoardViewModel
    {
        public BoardModel boardModel { get; private set; }
        public Button[,] Buttons { get; private set; }
        public bool BoardInitialized { get; private set; } = false;

        public event Action GameOver; // Game Over 이벤트

        public BoardViewModel()
        {
            boardModel = new BoardModel();
            Buttons = new Button[boardModel.rows, boardModel.cols];
        }

        public void OnCellClicked(int row, int col)
        {
            if (!BoardInitialized)
            {
                boardModel.Initialize(row, col);
                BoardInitialized = true;
            }

            RevealCell(row, col);
        }

        private void RevealCell(int row, int col)
        {
            var btn = Buttons[row, col];
            if (!btn.IsEnabled) return;

            btn.IsEnabled = false;
            btn.BackgroundColor = Tizen.NUI.Color.White;

            int value = boardModel.Cells[row, col];
            btn.Text = value == -1 ? "*" : value.ToString();
            btn.TextColor = Tizen.NUI.Color.Black;

            if (value == -1)
            {
                GameOver?.Invoke();
                return;
            }

            if (value == 0)
            {
                for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int nr = row + dr;
                        int nc = col + dc;
                        if (nr >= 0 && nr < boardModel.rows && nc >= 0 && nc < boardModel.cols)
                            RevealCell(nr, nc);
                    }
            }
        }
    }
}

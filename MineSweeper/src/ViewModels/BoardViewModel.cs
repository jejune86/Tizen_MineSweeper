using System;
using Tizen.NUI.Components;
using MineSweeper.Models;
using Tizen.Network.Nfc;

namespace MineSweeper.ViewModels
{
    public class BoardViewModel
    {
        public Board board { get; private set; }
        public Button[,] Buttons { get; private set; }
        public bool BoardInitialized { get; private set; } = false;

        public event Action GameOver; // Game Over 이벤트

        public BoardViewModel()
        {
            board = new Board();
            Buttons = new Button[board.rows, board.cols];
            BoardInitialized = false;
        }

        public void OnCellClicked(int row, int col)
        {
            if (!BoardInitialized)
            {
                board.Initialize(row, col);
                BoardInitialized = true;
            }

            if (CheckChord(row, col))
                RevealSurround(row, col);
            else
                RevealCell(row, col);
        }

        public void OnCellFlagged(int row, int col)
        {
            // 깃발 표시 토글
            if (!board.Cells[row, col].isRevealed)
            {
                board.Cells[row, col].isFlagged = !board.Cells[row, col].isFlagged;

                // 버튼 UI도 갱신
                Buttons[row, col].Text = board.Cells[row, col].isFlagged ? "🚩" : "";
            }
        }

        private bool CheckChord(int row, int col)
        {
            Cell cell = board.Cells[row, col];
            if (!cell.isRevealed || cell.num <= 0) return false;

            int flagCount = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr;
                    int nc = col + dc;
                    if (nr >= 0 && nr < board.rows && nc >= 0 && nc < board.cols && board.Cells[nr, nc].isFlagged)
                    {
                        flagCount++;
                    }
                }
            if (flagCount == board.Cells[row, col].num) return true;

            return false;
        }

        private void RevealSurround(int row, int col)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr;
                    int nc = col + dc;
                    if (nr >= 0 && nr < board.rows && nc >= 0 && nc < board.cols)
                        RevealCell(nr, nc);
                }
        }

        private void RevealCell(int row, int col) // TODO : BFS로 바꾸기
        {
            var btn = Buttons[row, col];
            int value = board.Cells[row, col].num;


            if (board.Cells[row, col].isFlagged || board.Cells[row, col].isRevealed) return;

            if (!board.Cells[row, col].isRevealed)
            {
                btn.BackgroundColor = Tizen.NUI.Color.White;
                board.Cells[row, col].isRevealed = true;
                btn.Text = value == -1 ? "*" : value.ToString();
                btn.TextColor = Tizen.NUI.Color.Black;
            }

            if (value == -1)
            {
                GameOver?.Invoke();
                return;
            }

            if (value == 0)
            {
                RevealSurround(row, col);
            }
        }
    }
}

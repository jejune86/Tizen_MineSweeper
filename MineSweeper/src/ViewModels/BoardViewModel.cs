using System;
using Tizen;
using Tizen.NUI.Components;
using MineSweeper.Models;
using Tizen.Network.Nfc;
using System.ComponentModel;

namespace MineSweeper.ViewModels
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        public Board board { get; private set; }
        public CellViewModel[,] Cells { get; private set; }
        public bool BoardInitialized { get; private set; } = false;
        private int remainingFlags;

        public int RemainingFlags
        {
            get => remainingFlags;
            set
            {
                if (remainingFlags != value)
                {
                    remainingFlags = value;
                    OnPropertyChanged(nameof(RemainingFlags));
                }
            }
        }
        private int leftCellToOpenCount;
        public int LeftCellToOpenCount
        {
            get => LeftCellToOpenCount;
            set
            {
                if (leftCellToOpenCount != value)
                {
                    leftCellToOpenCount = value;
                    OnPropertyChanged(nameof(LeftCellToOpenCount));
                }
            }
        }

        public event Action GameOver; // Game Over 이벤트

        public event Action GameClear;

        public BoardViewModel()
        {
            board = new Board();
            Cells = new CellViewModel[board.rows, board.cols];
            InitializeBoard();
            
        }

        public void InitializeBoard()
        {
            BoardInitialized = false;
            for (int r = 0; r < board.rows; r++)
                for (int c = 0; c < board.cols; c++)
                    if (Cells[r, c] == null)
                    {
                        Cells[r, c] = new CellViewModel
                        {
                            cell = new Cell(r, c)
                        };
                    }
                    else
                    {
                        Cells[r, c].IsRevealed = false;
                        Cells[r, c].IsFlagged = false;
                    }
            RemainingFlags = board.mineCount;
            leftCellToOpenCount = board.rows*board.cols - board.mineCount;
        }

        private void SetCells()
        {
            for (int r = 0; r < board.rows; r++)
                for (int c = 0; c < board.cols; c++)
                    Cells[r, c].cell.value = board.Cells[r,c].value;
        }

        public void OnCellClicked(int row, int col)
        {
            if (!BoardInitialized)
            {
                board.Initialize(row, col);
                BoardInitialized = true;
                SetCells();
                Log.Info("MineSweeper", $"Board Initialized");
            }

            if (CheckChord(row, col))
                RevealSurround(row, col);
            else
                RevealCell(row, col);
        }

        public void OnCellFlagged(int row, int col)
        {
            // 깃발 표시 토글
            var cell = Cells[row, col];
            if (!cell.IsRevealed)
            {
                cell.IsFlagged = !cell.IsFlagged; // 이 순간 PropertyChanged 발생 → View에서 자동 UI 갱신
                RemainingFlags = cell.IsFlagged ? RemainingFlags - 1 : RemainingFlags + 1;
            }
        }

        private bool CheckChord(int row, int col)
        {
            Log.Info("MineSweeper", $"ChekChord called : row={row}, col={col}");
            var cell = Cells[row, col];
            if (!cell.IsRevealed || cell.cell.value <= 0) return false;

            int flagCount = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr;
                    int nc = col + dc;
                    if (nr >= 0 && nr < board.rows && nc >= 0 && nc < board.cols && Cells[nr,nc].IsFlagged)
                    {
                        flagCount++;
                    }
                }
            if (flagCount == cell.cell.value) return true;

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
            var cell = Cells[row, col];

            if (cell.IsFlagged || cell.IsRevealed) return;

            cell.IsRevealed = true;
            cell.cell.value = board.Cells[row, col].value;
            leftCellToOpenCount--;

            if (cell.cell.value == -1)
            {
                GameOver?.Invoke();
                return;
            }

            if (leftCellToOpenCount == 0)
            {
                GameClear?.Invoke();
                return;
            }

            if (cell.cell.value == 0)
            {
                RevealSurround(row, col);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

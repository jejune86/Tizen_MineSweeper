using System;
using Tizen;
using Tizen.NUI.Components;
using MineSweeper.Models;
using MineSweeper.Services;
using System.ComponentModel;
using Tizen.NUI;

namespace MineSweeper.ViewModels
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private Board board;
        private CellViewModel[,] cells;
        public bool BoardInitialized { get; private set; } = false;

        // 필요한 속성만 노출
        public int Rows => board.rows;
        public int Cols => board.cols;
        public CellViewModel GetCell(int row, int col) => cells[row, col];
        private int remainingFlags;
        private int leftCellToOpenCount;
        private readonly GameTimerService gameTimerService;

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
       
        public int LeftCellToOpenCount
        {
            get => leftCellToOpenCount;
            set
            {
                if (leftCellToOpenCount != value)
                {
                    leftCellToOpenCount = value;
                    OnPropertyChanged(nameof(LeftCellToOpenCount));
                }
            }
        }

        public int ElapsedTime
        {
            get => gameTimerService?.ElapsedTime ?? 0;
        }


        public event Action GameOver; // Game Over 이벤트

        public event Action GameClear;

        /// <summary>
        /// BoardViewModel 생성자
        /// </summary>
        /// <param name="timerService">게임 타이머 서비스 (Dependency Injection)</param>
        public BoardViewModel(GameTimerService timerService = null)
        {
            board = new Board();
            cells = new CellViewModel[board.rows, board.cols];
            
            // Dependency Injection: GameTimerService 주입
            gameTimerService = timerService ?? new GameTimerService();
            gameTimerService.TimeUpdated += (time) => OnPropertyChanged(nameof(ElapsedTime));
            
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            BoardInitialized = false;
            gameTimerService.Reset();

            for (int r = 0; r < board.rows; r++)
                for (int c = 0; c < board.cols; c++)
                    if (cells[r, c] == null)
                    {
                        cells[r, c] = new CellViewModel(new Cell(r, c));
                    }
                    else
                    {
                        cells[r, c].IsRevealed = false;
                        cells[r, c].IsFlagged = false;
                    }
            RemainingFlags = board.mineCount;
            LeftCellToOpenCount = board.rows*board.cols - board.mineCount;
        }

        private void SetCells()
        {
            for (int r = 0; r < board.rows; r++)
                for (int c = 0; c < board.cols; c++)
                    cells[r, c].SetCellValue(board.Cells[r,c].value);
        }

        public void OnCellClicked(int row, int col)
        {
            if (!BoardInitialized)
            {
                board.Initialize(row, col);
                BoardInitialized = true;
                SetCells();
                gameTimerService.Start();
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
            var cell = cells[row, col];
            if (!cell.IsRevealed)
            {
                cell.IsFlagged = !cell.IsFlagged; // 이 순간 PropertyChanged 발생 → View에서 자동 UI 갱신
                RemainingFlags = cell.IsFlagged ? RemainingFlags - 1 : RemainingFlags + 1;
            }
        }

        private bool CheckChord(int row, int col)
        {
            Log.Info("MineSweeper", $"ChekChord called : row={row}, col={col}");
            var cell = cells[row, col];
            if (!cell.IsRevealed || cell.Value <= 0) return false;

            int flagCount = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr;
                    int nc = col + dc;
                    if (nr >= 0 && nr < board.rows && nc >= 0 && nc < board.cols && cells[nr,nc].IsFlagged)
                    {
                        flagCount++;
                    }
                }
            if (flagCount == cell.Value) return true;

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
            var cell = cells[row, col];

            if (cell.IsFlagged || cell.IsRevealed) return;

            cell.IsRevealed = true;
            cell.SetCellValue(board.Cells[row, col].value);
            LeftCellToOpenCount--;

            if (cell.Value == -1)
            {
                gameTimerService.Stop();
                GameOver?.Invoke();
                return;
            }

            if (leftCellToOpenCount == 0)
            {
                gameTimerService.Stop();
                GameClear?.Invoke();
                return;
            }

            if (cell.Value == 0)
            {
                RevealSurround(row, col);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

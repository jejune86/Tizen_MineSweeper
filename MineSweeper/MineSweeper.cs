using System;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace MineSweeper
{
    class Program : NUIApplication
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int MINE_COUNT = 15;

        public int revealCount;

        private int[,] board;
        private Button[,] buttons;
        private bool boardInitialized = false;

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        private void InitializeBoard(int firstRow, int firstCol)
        {
            revealCount = 0;
            board = new int[ROWS, COLS];
            Random rnd = new Random();
            int placedMines = 0;

            // 지뢰 배치 (첫 클릭 제외)
            while (placedMines < MINE_COUNT)
            {
                int r = rnd.Next(ROWS);
                int c = rnd.Next(COLS);
                if ((r == firstRow && c == firstCol) || board[r, c] == -1) continue;
                board[r, c] = -1;
                placedMines++;
            }

            // 주변 지뢰 수 계산
            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    if (board[r, c] == -1) continue;
                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = r + dr;
                            int nc = c + dc;
                            if (nr >= 0 && nr < ROWS && nc >= 0 && nc < COLS)
                                if (board[nr, nc] == -1) count++;
                        }
                    board[r, c] = count;
                }
            }
        }

        private void Initialize()
        {
            Window.Instance.KeyEvent += OnKeyEvent;

            ContentPage contentPage = new ContentPage
            {
                AppBar = new AppBar() { Title = "Mine Sweeper" }
            };

            View boardView = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new GridLayout
                {
                    Rows = ROWS,
                    Columns = COLS,
                    GridOrientation = GridLayout.Orientation.Horizontal
                }
            };

            buttons = new Button[ROWS, COLS];
            int width = Window.Instance.Size.Width / ROWS;

            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    Button btn = new Button
                    {
                        WidthSpecification = width,
                        HeightSpecification = width,
                        BorderlineWidth = 1,
                        BorderlineColor = Color.Black
                    };

                    int rr = r, cc = c; // 클로저를 위해
                    btn.Clicked += (s, e) => OnButtonClicked(rr, cc);

                    buttons[r, c] = btn;
                    boardView.Add(btn);
                }
            }

            contentPage.Content = boardView;
            Window.Instance.GetDefaultLayer().Add(contentPage);
        }

        private void OnButtonClicked(int row, int col)
        {
            if (!boardInitialized)
            {
                InitializeBoard(row, col);
                boardInitialized = true;
            }

            RevealCell(row, col);
        }

        private void RevealCell(int row, int col)
        {
            revealCount++;
            Button btn = buttons[row, col];
            if (!btn.IsEnabled) return; // 이미 열렸으면 무시
            btn.IsEnabled = false;
            btn.BackgroundColor = Color.White;

            int value = board[row, col];
            btn.Text = value == -1 ? "*" : value.ToString();
            btn.TextColor = Color.Black;
            if (value == -1)
            {
                // 지뢰 클릭 -> 게임 종료
                ShowGameOver();
                return;
            }

            if (value == 0)
            {
                for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int nr = row + dr;
                        int nc = col + dc;
                        if (nr >= 0 && nr < ROWS && nc >= 0 && nc < COLS)
                            RevealCell(nr, nc);
                    }
            }
        }

        private void ShowGameOver()
        {
            // 모든 버튼 비활성화
            for (int r = 0; r < ROWS; r++)
                for (int c = 0; c < COLS; c++)
                    buttons[r, c].IsEnabled = false;

            Console.WriteLine("Game Over!"); // 나중에 NUI 레이블로 표시 가능
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down &&
                (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}

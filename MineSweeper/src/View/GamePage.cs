using System;
using MineSweeper.ViewModels;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;


namespace MineSweeper.Views
{
    public class GamePage : ContentPage
    {
        private BoardViewModel boardViewModel;
        private int cellSize;
        private Timer longPressTimer;

        public GamePage(BoardViewModel vm)
        {
            boardViewModel = vm;
            AppBar = new AppBar()
            {
                Title = "Game"
            };

            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.MatchParent;

            var boardLayout = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new GridLayout
                {
                    Rows = boardViewModel.board.rows,     // 인스턴스로 접근
                    Columns = boardViewModel.board.cols,
                    GridOrientation = GridLayout.Orientation.Horizontal
                }
            };

            cellSize = Window.Instance.Size.Width / boardViewModel.board.rows;

            for (int r = 0; r < boardViewModel.board.rows; r++)
            {
                for (int c = 0; c < boardViewModel.board.cols; c++)
                {
                    var btn = new Button
                    {
                        WidthSpecification = cellSize,
                        HeightSpecification = cellSize,
                        BorderlineWidth = 1,
                        BorderlineColor = Color.Black
                    };

                    int rr = r, cc = c;
                    
                    btn.TouchEvent += (s, e) => OnCellTouchEvent(s, e, rr, cc);

                    boardViewModel.Buttons[r, c] = btn;
                    boardLayout.Add(btn);
                }
            }


            Content = boardLayout;

            boardViewModel.GameOver += () =>
            {
                for (int r = 0; r < boardViewModel.board.rows; r++)
                    for (int c = 0; c < boardViewModel.board.cols; c++)
                        boardViewModel.Buttons[r, c].IsEnabled = false;
            };
        }

        private bool OnCellTouchEvent(object sender, TouchEventArgs e, int row, int col)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Log.Info("MineSweeper", $"Pressed Down at ({row}, {col})");

                // 0.6초(600ms) 이상 누르면 깃발 표시
                longPressTimer = new Timer(600);
                longPressTimer.Tick += (s, args) =>
                {
                    boardViewModel.OnCellFlagged(row, col);
                    Log.Info("MineSweeper", $"Long Press Detected at ({row}, {col})");
                    longPressTimer.Stop();
                    longPressTimer.Dispose();
                    longPressTimer = null;
                    return false; // 한 번만 실행
                };
                longPressTimer.Start();
            }
            else if (e.Touch.GetState(0) == PointStateType.Up)
            {
                // 만약 long press 이전에 손을 뗐다면 -> 일반 클릭으로 처리
                if (longPressTimer != null)
                {
                    longPressTimer.Stop();
                    longPressTimer.Dispose();
                    longPressTimer = null;
                    boardViewModel.OnCellClicked(row, col);
                }
            }

            return true;
        }
    }
}

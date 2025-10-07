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
        private DateTime pressStartTime;

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

        private bool OnCellTouchEvent(Object sender, TouchEventArgs e, int row, int col)
        {   
            
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Log.Info("MineSweeper", $"Pressed Down at ({row}, {col})");
                pressStartTime = DateTime.Now;
            }
            else if (e.Touch.GetState(0) == PointStateType.Up)
            {
                var pressDuration = (DateTime.Now - pressStartTime).TotalMilliseconds;
                Log.Info("MineSweeper", $"Released ({row}, {col}), duration={pressDuration}ms");

                if (pressDuration > 600)
                {
                    boardViewModel.OnCellFlagged(row, col); // 길게 눌렀을 때
                }
                else
                {
                    boardViewModel.OnCellClicked(row, col); // 짧게 눌렀을 때
                }
            }

            return true;
        }
    }
}

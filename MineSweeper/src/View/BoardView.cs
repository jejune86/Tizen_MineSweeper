using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using MineSweeper.ViewModels;

namespace MineSweeper.Views
{
    public class BoardView : ContentPage
    {
        private BoardViewModel boardViewModel;
        private int cellSize;

        public BoardView(BoardViewModel vm)
        {
            boardViewModel = vm;
            AppBar.Title = "Mine Sweeper";

            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.MatchParent;

            var boardLayout = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new GridLayout
                {
                    Rows = boardViewModel.boardModel.rows,     // 인스턴스로 접근
                    Columns = boardViewModel.boardModel.cols,
                    GridOrientation = GridLayout.Orientation.Horizontal
                }
            };

            cellSize = Window.Instance.Size.Width / boardViewModel.boardModel.rows;

            for (int r = 0; r < boardViewModel.boardModel.rows; r++)
            {
                for (int c = 0; c < boardViewModel.boardModel.cols; c++)
                {
                    var btn = new Button
                    {
                        WidthSpecification = cellSize,
                        HeightSpecification = cellSize,
                        BorderlineWidth = 1,
                        BorderlineColor = Tizen.NUI.Color.Black
                    };

                    int rr = r, cc = c;
                    btn.Clicked += (s, e) => boardViewModel.OnCellClicked(rr, cc);

                    boardViewModel.Buttons[r, c] = btn;
                    boardLayout.Add(btn);
                }
            }


            Content = boardLayout;

            boardViewModel.GameOver += () =>
            {
                for (int r = 0; r < boardViewModel.boardModel.rows; r++)
                    for (int c = 0; c < boardViewModel.boardModel.cols; c++)
                        boardViewModel.Buttons[r, c].IsEnabled = false;
            };
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
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

        private CellButton[,] buttons;

        private InfoBar infoBar;

        public GamePage(BoardViewModel vm)
        {
            boardViewModel = vm;
            // AppBar = new AppBar()
            // {
            //     Title = "Game"
            // };

            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.MatchParent;

            InitializeContent();

            boardViewModel.GameOver += () =>
            {
                for (int r = 0; r < boardViewModel.board.rows; r++)
                    for (int c = 0; c < boardViewModel.board.cols; c++)
                    {
                        buttons[r, c].IsEnabled = false;
                    }
                infoBar.UpdateFace(1);

            };

            boardViewModel.GameClear += () =>
            {
                for (int r = 0; r < boardViewModel.board.rows; r++)
                    for (int c = 0; c < boardViewModel.board.cols; c++)
                    {
                        buttons[r, c].IsEnabled = false;
                    }
                infoBar.UpdateFace(2);
            };
        }

        private void InitializeContent()
        {
            var boardLayout = InitializeBoardLayout();

            infoBar = new InfoBar(boardViewModel);

            Content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
            };

            var underBoard = new View()
            {
                BackgroundImage = ImagePaths.UnderBoard,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
            };

            Button replay = new Button()
            {
                WidthSpecification = 100,
                HeightSpecification = 100,
                BackgroundImage = ImagePaths.GetCellImage("redo"),
                BoxShadow = new Shadow(5, new Tizen.NUI.Color(0, 0, 0, 0.4f)) 
            };

            replay.Clicked += OnReplayClicked;

            underBoard.Add(replay);

            Content.Add(infoBar);
            Content.Add(boardLayout);
            Content.Add(underBoard);
        }

        private View InitializeBoardLayout()
        {
            buttons = new CellButton[boardViewModel.board.rows, boardViewModel.board.cols];

            int boardSize = Window.Instance.Size.Width;
            var boardLayout = new View
            {
                WidthSpecification = boardSize,
                HeightSpecification = boardSize,
                Layout = new GridLayout
                {
                    Rows = boardViewModel.board.rows,     // 인스턴스로 접근
                    Columns = boardViewModel.board.cols,
                    GridOrientation = GridLayout.Orientation.Horizontal
                },
                BackgroundColor = new Tizen.NUI.Color(0.349f, 0.349f, 0.349f, 1f)
            };

            cellSize = Window.Instance.Size.Width / boardViewModel.board.rows;

            for (int r = 0; r < boardViewModel.board.rows; r++)
            {
                for (int c = 0; c < boardViewModel.board.cols; c++)
                {

                    var cellVM = boardViewModel.Cells[r, c];
                    var btn = new CellButton(cellSize, cellVM);

                    btn.CellTouched += (row, col, isLongPress) =>
                    {
                        if (isLongPress)
                            boardViewModel.OnCellFlagged(row, col);
                        else
                            boardViewModel.OnCellClicked(row, col);
                    };
                    
                    buttons[r, c] = btn; // ✅ View가 직접 관리
                    boardLayout.Add(btn);
                }
            }
            return boardLayout;
        }

        

        private void OnReplayClicked(object sender, ClickedEventArgs e)
        {
            boardViewModel.InitializeBoard();
            for (int r = 0; r < boardViewModel.board.rows; r++)
                for (int c = 0; c < boardViewModel.board.cols; c++)
                    buttons[r, c].IsEnabled = true;
            infoBar.UpdateFace(0);
        }


    }
    
    
}

using System;
using System.ComponentModel;
using System.Drawing;
using MineSweeper.Views;
using MineSweeper.ViewModels;
using MineSweeper.Views.Effects;
using MineSweeper.Services;
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

        private View effectsOverlay; // 폭죽 효과를 표시할 오버레이 뷰
        private FireworksEffectManager fireworksEffectManager;

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
                for (int r = 0; r < boardViewModel.Rows; r++)
                    for (int c = 0; c < boardViewModel.Cols; c++)
                    {
                        buttons[r, c].IsEnabled = false;
                    }
                infoBar.UpdateFace(1);

            };

            boardViewModel.GameClear += () =>
            {
                for (int r = 0; r < boardViewModel.Rows; r++)
                    for (int c = 0; c < boardViewModel.Cols; c++)
                    {
                        buttons[r, c].IsEnabled = false;
                    }
                infoBar.UpdateFace(2);
                fireworksEffectManager.ShowFireworks(15);
            };
        }

        private void InitializeContent()
        {
            var boardLayout = InitializeBoardLayout();

            infoBar = new InfoBar(boardViewModel);

            var mainLayout = new View()
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

            mainLayout.Add(infoBar);
            mainLayout.Add(boardLayout);
            mainLayout.Add(underBoard);

            effectsOverlay = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                // 기본 레이아웃(Absolute)을 사용하므로 자식 뷰의 Position을 직접 제어
            };

            // 5. Content를 루트 View로 설정하고 mainLayout과 effectsOverlay를 추가
            Content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            Content.Add(mainLayout);
            Content.Add(effectsOverlay); // mainLayout 위에 effectsOverlay가 겹쳐짐

            // FireworksEffectManager 초기화
            fireworksEffectManager = new FireworksEffectManager(effectsOverlay);
        }

        private View InitializeBoardLayout()
        {
            buttons = new CellButton[boardViewModel.Rows, boardViewModel.Cols];

            int boardSize = Window.Instance.Size.Width;
            var boardLayout = new View
            {
                WidthSpecification = boardSize,
                HeightSpecification = boardSize,
                Layout = new GridLayout
                {
                    Rows = boardViewModel.Rows,
                    Columns = boardViewModel.Cols,
                    GridOrientation = GridLayout.Orientation.Horizontal
                },
                BackgroundColor = new Tizen.NUI.Color(0.349f, 0.349f, 0.349f, 1f)
            };

            cellSize = Window.Instance.Size.Width / boardViewModel.Rows;

            for (int r = 0; r < boardViewModel.Rows; r++)
            {
                for (int c = 0; c < boardViewModel.Cols; c++)
                {

                    var cellVM = boardViewModel.GetCell(r, c);
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
            for (int r = 0; r < boardViewModel.Rows; r++)
                for (int c = 0; c < boardViewModel.Cols; c++)
                    buttons[r, c].IsEnabled = true;
            infoBar.UpdateFace(0);
        }

    }
    
    
}

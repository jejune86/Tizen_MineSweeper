using System;
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

        private Timer longPressTimer;

        private CellButton[,] buttons;

        public GamePage(BoardViewModel vm)
        {
            boardViewModel = vm;
            AppBar = new AppBar()
            {
                Title = "Game"
            };

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

            };
        }

        private void InitializeContent()
        {
            var boardLayout = InitializeBoardLayout();

            var infoBar = new InfoBar(boardViewModel);

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
                BackgroundImage = ImagePaths.GetCellImage("redo")
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
                    var btn = new CellButton(cellSize);

                    int rr = r, cc = c;

                    btn.TouchEvent += (s, e) => OnCellTouchEvent(s, e, rr, cc);

                    var cellVM = boardViewModel.Cells[r, c];
                    cellVM.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(CellViewModel.IsFlagged))
                        {
                            Log.Info("MineSweeper", $"Flag Change Detected");
                            btn.ToggleFlag();
                        }

                    };

                    cellVM.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(CellViewModel.IsRevealed))
                        {
                            Log.Info("MineSweeper", $"Reveal Change Detected");
                            try
                            {
                                if (cellVM.cell.isRevealed)
                                {
                                    btn.OpenCell(cellVM.cell.value);
                                }
                                else
                                {
                                    btn.ResetCell();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("MineSweeper", $"Navigation error: {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                    };
                    buttons[r, c] = btn; // ✅ View가 직접 관리
                    boardLayout.Add(btn);
                }
            }
            return boardLayout;
        }

        private bool OnCellTouchEvent(object sender, TouchEventArgs e, int row, int col)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Log.Info("MineSweeper", $"Pressed Down at ({row}, {col})");

                if (!boardViewModel.Cells[row, col].cell.isFlagged && !boardViewModel.Cells[row, col].cell.isRevealed)
                    buttons[row, col].BackgroundImage = ImagePaths.GetCellImage(0);

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
                if (!boardViewModel.Cells[row, col].cell.isFlagged && !boardViewModel.Cells[row, col].cell.isRevealed)
                    buttons[row, col].BackgroundImage = ImagePaths.GetCellImage("close");
                // 만약 long press 이전에 손을 뗐다면 -> 일반 클릭으로 처리
                if (longPressTimer != null)
                {
                    longPressTimer.Stop();
                    longPressTimer.Dispose();
                    longPressTimer = null;
                    try
                    {
                        boardViewModel.OnCellClicked(row, col);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MineSweeper", $"Cell Click Error: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }

            return true;
        }

        private void OnReplayClicked(object sender, ClickedEventArgs e)
        {
            boardViewModel.InitializeBoard();
            for (int r = 0; r < boardViewModel.board.rows; r++)
                for (int c = 0; c < boardViewModel.board.cols; c++)
                    buttons[r, c].IsEnabled = true;
        }
    }
}

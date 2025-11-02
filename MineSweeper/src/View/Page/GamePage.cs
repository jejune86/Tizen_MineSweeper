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

        private View effectsOverlay; // 1. 폭죽 효과를 표시할 오버레이 뷰
        private Random effectsRand = new Random(); // 2. 랜덤 위치용 Random 인스턴스

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
                ShowFireworksEffect(15);
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
        


        private void ShowFireworksEffect(int count = 5)
        {
            for (int i = 0; i < count; i++)
            {
                // 각 폭죽이 약간의 시간 차이를 두고 터지도록 Delay를 줍니다.
                int delay = i * effectsRand.Next(100, 300); 
                CreateFirework(delay);
            }
        }

        /// <summary>
        /// 단일 폭죽 ImageView를 생성하고 애니메이션을 적용합니다.
        /// </summary>
        private void CreateFirework(int startDelay)
    {
            var fireworkImage = new ImageView()
            {
                // TODO: 'ImagePaths.Firework'를 실제 폭죽 이미지 경로로 변경하세요.
                ResourceUrl = ImagePaths.GetImage("firework"),
                Size = new Tizen.NUI.Size(300, 300), // 폭죽 크기
                Opacity = 0.0f, // 처음엔 투명
                Scale = new Vector3(0.1f, 0.1f, 1.0f), // 처음엔 작게
            };

            // 화면 내 랜덤 위치 지정 (InfoBar 아래, 하단 영역 위)
            int x = effectsRand.Next(0, Window.Instance.Size.Width - 150);
            int y = effectsRand.Next((int)infoBar.Size.Height, Window.Instance.Size.Height - 150);
            fireworkImage.Position = new Position(x, y);

            // 오버레이에 추가
            effectsOverlay.Add(fireworkImage);

            // ✅ 수정 1: Timer를 사용하여 애니메이션 시작을 지연시킵니다.
            var timer = new Tizen.NUI.Timer((uint)startDelay);
            timer.Tick += (s, e) =>
            {
                // 애니메이션 생성 (총 2초)
                var animation = new Animation(2000);

                // ✅ 수정 2: AlphaFunction.BuiltinFunctions.EaseOut 사용
                animation.DefaultAlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut);

                // 1. 나타나면서 커지기 (0ms ~ 800ms)
                // AnimateTo(타겟, 속성, 목표값, 시작시간, 지속시간)
                animation.AnimateTo(fireworkImage, "Opacity", 1.0f, 0, 300); // 0.3초만에 불투명
                animation.AnimateTo(fireworkImage, "Scale", new Vector3(1.0f, 1.0f, 1.0f), 0, 800);

                // 2. 서서히 사라지기 (1000ms ~ 2000ms)
                animation.AnimateTo(fireworkImage, "Opacity", 0.0f, 1000, 1000);

                // 3. 애니메이션 종료 시 뷰 제거
                animation.Finished += (s_anim, e_anim) =>
                {
                    // 애니메이션이 끝난 뷰는 오버레이에서 제거
                    effectsOverlay.Remove(fireworkImage);
                    fireworkImage.Dispose(); // 메모리 정리
                };

                // 애니메이션 재생
                animation.Play();

                return false; // Timer를 1회만 실행하고 중지
            };

            timer.Start();
        }

    }
    
    
}

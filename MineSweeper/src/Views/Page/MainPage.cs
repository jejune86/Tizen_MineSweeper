using System;
using System.Net.Http.Headers;
using MineSweeper.ViewModels;
using MineSweeper.Views;
using MineSweeper.Services;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

public class MainPage : ContentPage
{
    private BoardViewModel boardViewModel;

    private Button playButton;

    private Button howToPlayButton;

    public MainPage()
    {
        // AppBar = new AppBar()
        // {
        //     Title = "💣 MineSweeper",
        //     BackgroundColor = new Color(0.12f, 0.12f, 0.12f, 1f)
        // };

        WidthSpecification = LayoutParamPolicies.MatchParent;
        HeightSpecification = LayoutParamPolicies.MatchParent;
        BackgroundImage = ImagePaths.GetImage("main_page");

        InitializeUI();
    }

    private void InitializeUI()
    {
        InitializeButtons();

        // 전체 레이아웃
        var layout = new View()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent,
            HeightSpecification = LayoutParamPolicies.MatchParent,
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                CellPadding = new Size2D(0, 40)
            },
            Padding = new Extents(60, 60, 100, 100)
        };

        layout.Add(playButton);
        layout.Add(howToPlayButton);

        Content = layout;
    }

    private void InitializeButtons()
    {   

        playButton = new Button()
        {
            WidthSpecification = 512,
            HeightSpecification = 128,
            BackgroundImage = ImagePaths.GetImage("play"),
            BoxShadow = new Shadow(10, new Color(0, 0, 0, 0.6f)),
            Opacity = 1.0f
        };
        playButton.Clicked += (s, e) =>
        {
            // Dependency Injection: GameTimerService 생성 후 주입
            var timerService = new GameTimerService();
            boardViewModel = new BoardViewModel(timerService);
            try
            {
                Navigator.Push(new GamePage(boardViewModel));
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("MineSweeper", $"Navigation error: {ex.Message}\n{ex.StackTrace}");
            }
        };

        // How to Play 버튼
        howToPlayButton = new Button()
        {
            WidthSpecification = 512,
            HeightSpecification = 128,
            BackgroundImage = ImagePaths.GetImage("how_to"),
            BoxShadow = new Shadow(10, new Color(0, 0, 0, 0.6f)),
            Opacity = 1.0f
        };
        howToPlayButton.Clicked += (s, e) =>
        {
            Navigator.Push(new HowToPlayPage());
        };

        // Hover / Focus 효과
        playButton.Focusable = true;
        playButton.FocusGained += (s, e) => playButton.Opacity = 0.8f;
        playButton.FocusLost += (s, e) => playButton.Opacity = 1.0f;

        howToPlayButton.Focusable = true;
        howToPlayButton.FocusGained += (s, e) => howToPlayButton.Opacity = 0.8f;
        howToPlayButton.FocusLost += (s, e) => howToPlayButton.Opacity = 1.0f;
    }
}

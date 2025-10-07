using System.Drawing;
using MineSweeper.ViewModels;
using MineSweeper.Views;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

public class MainPage : ContentPage
{
    private BoardViewModel boardViewModel;
    public MainPage()
    {
        AppBar = new AppBar()
        {
            Title = "My Game"
        };

        // 버튼 생성
        Button playButton = new Button()
        {
            Text = "Play",
            TextColor = Tizen.NUI.Color.Black,
            WidthSpecification = LayoutParamPolicies.MatchParent,
            BackgroundColor = Tizen.NUI.Color.Green,
        };
        playButton.Clicked += (s, e) =>
        {
            boardViewModel = new BoardViewModel();
            Navigator.Push(new GamePage(boardViewModel));
        };

        Button howToPlayButton = new Button()
        {
            Text = "How to Play",
            TextColor = Tizen.NUI.Color.Black,
            WidthSpecification = LayoutParamPolicies.MatchParent,
            BackgroundColor = Tizen.NUI.Color.Yellow
        };
        howToPlayButton.Clicked += (s, e) =>
        {
            Navigator.Push(new HowToPlayPage());
        };

        View content = new View()
        {   
            WidthSpecification = LayoutParamPolicies.MatchParent,
            HeightSpecification = LayoutParamPolicies.MatchParent,
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            },
        };

        content.Add(playButton);
        content.Add(howToPlayButton);

        Content = content;
    }
}

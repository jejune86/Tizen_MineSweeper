using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

public class HowToPlayPage : ContentPage
{
    public HowToPlayPage()
    {
        AppBar = new AppBar()
        {
            Title = "How to Play"
        };

        TextLabel label = new TextLabel("여기에 게임 방법 설명 넣기")
        {
            MultiLine = true,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            PointSize = 15
        };

        Content = label;
    }
}

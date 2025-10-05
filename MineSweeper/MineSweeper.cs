using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

class Program : NUIApplication
{
    protected override void OnCreate()
    {
        base.OnCreate();

        // Navigator를 Window에 붙임
        Navigator navigator = new Navigator()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent,
            HeightSpecification = LayoutParamPolicies.MatchParent
        };
        Window.Instance.Add(navigator);

        // 첫 페이지로 MainPage 로드
        navigator.Push(new MainPage());
    }

    static void Main(string[] args)
    {
        var app = new Program();
        app.Run(args);
    }
}
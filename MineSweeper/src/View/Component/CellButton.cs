using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;

public class CellButton : Button
{
    public bool IsOpened { get; private set; } = false;
    public bool IsFlagged { get; private set; } = false;
    public int Row { get; set; }
    public int Col { get; set; }

    private Color closedColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
    private Color openedColor = new Color(1f, 1f, 1f, 1f);
    private Color borderColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);

    public CellButton(int cellSize)
    {
        WidthSpecification = cellSize;
        HeightSpecification = cellSize;

        CornerRadius = 8.0f; // 둥근 모서리
        BorderlineWidth = 2.0f;
        BorderlineColor = borderColor;
        BackgroundColor = closedColor;

        Focusable = true;
        FocusableInTouch = true;
    }

    public void OpenCell(int value)
    {
        if (IsOpened) return;
        IsOpened = true;
        BackgroundColor = openedColor;
        BorderlineColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        if (value == -1) Text = "💣";
        else if (value != 0)
        {
            Text = value.ToString();
            TextColor = new Color(0, 0, 0, 1);
        }
    }

    public void ToggleFlag()
    {
        IsFlagged = !IsFlagged;

        if (IsFlagged)
        {
            BackgroundColor = new Color(1f, 0.7f, 0.7f, 1f);
            Text = "🚩";
        }
        else
        {
            Text = "";
            BackgroundColor = closedColor;
        }
    }
}
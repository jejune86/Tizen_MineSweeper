using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;


public class CellButton : Button
{
    public bool IsOpened { get; private set; } = false;
    public bool IsFlagged { get; private set; } = false;
    public int Row { get; set; }
    public int Col { get; set; }

    public CellButton(int cellSize)
    {
        WidthSpecification = cellSize;
        HeightSpecification = cellSize;

        BackgroundImage = ImagePaths.CELL_CLOSE;

        Focusable = true;
        FocusableInTouch = true;
    }

    public void OpenCell(int value)
    {
        if (IsOpened) return;
        IsOpened = true;


        if (value == -1)
        {
            BackgroundImage = ImagePaths.CELL_MINE;
        }
        else
        {
            BackgroundImage = ImagePaths.GetCellImage(value);
        }
    }

    public void ResetCell()
    {
        IsOpened = false;
        BackgroundImage = ImagePaths.CELL_CLOSE;
    }

    public void ToggleFlag()
    {
        IsFlagged = !IsFlagged;

        if (IsFlagged)
        {
            BackgroundImage = ImagePaths.CELL_FLAG;
        }
        else
        {
            BackgroundImage = ImagePaths.CELL_CLOSE;
        }
    }
}

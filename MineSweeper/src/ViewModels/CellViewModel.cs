using System.ComponentModel;
using MineSweeper.Models;

public class CellViewModel : INotifyPropertyChanged
{
    public Cell cell;

    public bool IsFlagged
    {
        get => cell.isFlagged;
        set
        {
            if (cell.isFlagged != value)
            {
                cell.isFlagged = value;
                OnPropertyChanged(nameof(IsFlagged)); 
            }
        }
    }

    public bool IsRevealed
    {
        get => cell.isRevealed;
        set
        {
            if (cell.isRevealed != value)
            {
                cell.isRevealed = value;
                OnPropertyChanged(nameof(IsRevealed));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

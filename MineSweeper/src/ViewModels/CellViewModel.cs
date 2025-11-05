using System.ComponentModel;
using MineSweeper.Models;

namespace MineSweeper.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        private Cell cell;

        public int Row => cell.row;
        public int Col => cell.col;
        public int Value
        {
            get => cell.value;
            set
            {
                if (cell.value != value)
                {
                    cell.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

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

        // 내부에서만 사용할 수 있도록 internal 생성자 또는 SetCell 메서드
        internal CellViewModel(Cell cell)
        {
            this.cell = cell;
        }

        // BoardViewModel에서 사용할 수 있도록 internal setter
        internal void SetCellValue(int value)
        {
            this.cell.value = value;
            OnPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System.ComponentModel;
using Tizen;
using System;
using MineSweeper.ViewModels;

namespace MineSweeper.Views
{
    public class CellButton : Button
    {
        private Timer longPressTimer;
        public event Action<int, int, bool> CellTouched;

        private CellViewModel cellViewModel;

        public CellButton(int cellSize, CellViewModel cellViewModel)
        {
            WidthSpecification = cellSize;
            HeightSpecification = cellSize;

            BackgroundImage = ImagePaths.CELL_CLOSE;

            this.cellViewModel = cellViewModel;
            this.cellViewModel.PropertyChanged += OnCellPropertyChanged;

            TouchEvent += OnTouch;
        }

        public void OpenCell(int value)
        {
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
            BackgroundImage = ImagePaths.CELL_CLOSE;
        }

        public void ToggleFlag()
        {
            if (cellViewModel.IsFlagged)
            {
                BackgroundImage = ImagePaths.CELL_FLAG;
            }
            else
            {
                BackgroundImage = ImagePaths.CELL_CLOSE;
            }
        }

        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            Log.Info("MineSweeper", $"PropertyChanged fired: {e.PropertyName}");


            if (e.PropertyName == nameof(CellViewModel.IsFlagged))
            {
                Log.Info("MineSweeper", $"Flag Change Detected at ({cellViewModel.Row}, {cellViewModel.Col})");
                ToggleFlag();
            }
            else if (e.PropertyName == nameof(CellViewModel.IsRevealed))
            {
                Log.Info("MineSweeper", $"Reveal Change Detected at ({cellViewModel.Row}, {cellViewModel.Col})");
                try
                {
                    if (cellViewModel.IsRevealed)
                        OpenCell(cellViewModel.Value);
                    else
                        ResetCell();
                }
                catch (Exception ex)
                {
                    Log.Error("MineSweeper", $"Navigation error: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }


        private bool OnTouch(object sender, TouchEventArgs e)
        {

            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Log.Info("MineSweeper", $"Pressed Down at ({cellViewModel.Row}, {cellViewModel.Col})");

                if (!cellViewModel.IsFlagged && !cellViewModel.IsRevealed)
                    BackgroundImage = ImagePaths.GetCellImage(0);

                // 0.6초(600ms) 이상 누르면 깃발 표시
                longPressTimer = new Timer(600);
                longPressTimer.Tick += (s, args) =>
                {
                    CellTouched?.Invoke(cellViewModel.Row, cellViewModel.Col, true);
                    Log.Info("MineSweeper", $"Long Press at ({cellViewModel.Row}, {cellViewModel.Col})");
                    longPressTimer.Stop();
                    longPressTimer.Dispose();
                    longPressTimer = null;
                    return false; // 한 번만 실행
                };
                longPressTimer.Start();
            }
            else if (e.Touch.GetState(0) == PointStateType.Up)
            {
                if (!cellViewModel.IsFlagged && !cellViewModel.IsRevealed)
                    BackgroundImage = ImagePaths.GetCellImage("close");
                // 만약 long press 이전에 손을 뗐다면 -> 일반 클릭으로 처리
                if (longPressTimer != null)
                {
                    longPressTimer.Stop();
                    longPressTimer.Dispose();
                    longPressTimer = null;
                    try
                    {
                        CellTouched?.Invoke(cellViewModel.Row, cellViewModel.Col, false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MineSweeper", $"Cell Click Error: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }

            return true;
        }
    }
}

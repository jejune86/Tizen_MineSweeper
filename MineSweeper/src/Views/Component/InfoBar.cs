using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using MineSweeper.ViewModels;
using MineSweeper.Services;
using System;
using System.ComponentModel;
using Tizen.Messaging.Email;


namespace MineSweeper.Views
{
    public class InfoBar : View
    {
        private ImageView[] remainingFlagCountImage = new ImageView[3];
        private ImageView faceImage;
        private ImageView[] timerImages = new ImageView[3];
        private BoardViewModel boardViewModel;


        public InfoBar(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;

            int screenWidth = Window.Instance.Size.Width;
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = (int)(screenWidth * 48f / 320f);

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Padding = new Extents(30, 30, 20, 20);
            BackgroundImage = ImagePaths.InfoBoard;

            // --- 남은 지뢰 수 (숫자 이미지 3개) ---
            for (int i = 0; i < 3; i++)
            {
                remainingFlagCountImage[i] = new ImageView()
                {
                    ResourceUrl = ImagePaths.GetNumberImage(0), // 더미 0
                    WidthSpecification = 64,  // 숫자 이미지 크기
                    HeightSpecification = 96
                };
                Add(remainingFlagCountImage[i]);
            }

            // --- 가운데 웃는 얼굴 ---
            faceImage = new ImageView()
            {
                ResourceUrl = ImagePaths.GetCellImage("smile"),
                WidthSpecification = 90,
                HeightSpecification = 90,
                Margin = new Extents(40, 40, 0, 0)
            };
            Add(faceImage);

            // --- 경과 시간 (숫자 이미지 3개) ---
            for (int i = 0; i < 3; i++)
            {
                timerImages[i] = new ImageView()
                {
                    ResourceUrl = ImagePaths.GetNumberImage(0), // 더미 0
                    WidthSpecification = 64,
                    HeightSpecification = 96
                };
                Add(timerImages[i]);
            }

            boardViewModel.PropertyChanged += OnNumberChanged;
        }

        public void UpdateFace(int type)
        {
            if (type == 0) faceImage.ResourceUrl = ImagePaths.CELL_SMILE;
            else if (type == 1) faceImage.ResourceUrl = ImagePaths.CELL_DEAD;
            else faceImage.ResourceUrl = ImagePaths.CELL_SUNGLASS;
        }

        // 숫자 이미지 업데이트 함수
        private void UpdateNumberImages(ImageView[] images, int value)
        {
            string str = value.ToString("D3"); // 3자리로
            for (int i = 0; i < 3; i++)
            {
                images[i].ResourceUrl = ImagePaths.GetNumberImage(int.Parse(str[i].ToString()));
            }
        }


        private void OnNumberChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(boardViewModel.RemainingFlags))
            {
                UpdateNumberImages(remainingFlagCountImage, boardViewModel.RemainingFlags);
            }

            else if (e.PropertyName == nameof(boardViewModel.ElapsedTime))
            {
                UpdateNumberImages(timerImages, boardViewModel.ElapsedTime);
            }
        }

    }
}
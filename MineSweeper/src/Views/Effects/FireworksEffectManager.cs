using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using MineSweeper.Services;

namespace MineSweeper.Views.Effects
{
    /// <summary>
    /// 폭죽 효과를 관리하는 클래스
    /// </summary>
    public class FireworksEffectManager
    {
        private View effectsOverlay;
        private Random random;

        public FireworksEffectManager(View overlay)
        {
            effectsOverlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            random = new Random();
        }

        /// <summary>
        /// 여러 개의 폭죽 효과를 표시합니다.
        /// </summary>
        /// <param name="count">폭죽 개수</param>
        public void ShowFireworks(int count = 5)
        {
            for (int i = 0; i < count; i++)
            {
                // 각 폭죽이 약간의 시간 차이를 두고 터지도록 Delay를 줍니다.
                int delay = i * random.Next(100, 300);
                CreateFirework(delay);
            }
        }

        /// <summary>
        /// 단일 폭죽 ImageView를 생성하고 애니메이션을 적용합니다.
        /// </summary>
        /// <param name="startDelay">애니메이션 시작 지연 시간 (ms)</param>
        private void CreateFirework(int startDelay)
        {
            var fireworkImage = new ImageView()
            {
                ResourceUrl = ImagePaths.GetImage("firework"),
                Size = new Tizen.NUI.Size(300, 300), // 폭죽 크기
                Opacity = 0.0f, // 처음엔 투명
                Scale = new Vector3(0.1f, 0.1f, 1.0f), // 처음엔 작게
            };

            // 화면 내 랜덤 위치 지정
            int x = random.Next(0, Window.Instance.Size.Width - 150);
            int y = random.Next(0, Window.Instance.Size.Height - 150);
            fireworkImage.Position = new Position(x, y);

            // 오버레이에 추가
            effectsOverlay.Add(fireworkImage);

            // Timer를 사용하여 애니메이션 시작을 지연시킵니다.
            var timer = new Tizen.NUI.Timer((uint)startDelay);
            timer.Tick += (s, e) =>
            {
                // 애니메이션 생성 (총 2초)
                var animation = new Animation(2000);

                // AlphaFunction.BuiltinFunctions.EaseOut 사용
                animation.DefaultAlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut);

                // 1. 나타나면서 커지기 (0ms ~ 800ms)
                animation.AnimateTo(fireworkImage, "Opacity", 1.0f, 0, 300); // 0.3초만에 불투명
                animation.AnimateTo(fireworkImage, "Scale", new Vector3(1.0f, 1.0f, 1.0f), 0, 800);

                // 2. 서서히 사라지기 (1000ms ~ 2000ms)
                animation.AnimateTo(fireworkImage, "Opacity", 0.0f, 1000, 1000);

                // 3. 애니메이션 종료 시 뷰 제거
                animation.Finished += (s_anim, e_anim) =>
                {
                    // 애니메이션이 끝난 뷰는 오버레이에서 제거
                    effectsOverlay.Remove(fireworkImage);
                    fireworkImage.Dispose(); // 메모리 정리
                };

                // 애니메이션 재생
                animation.Play();

                return false; // Timer를 1회만 실행하고 중지
            };

            timer.Start();
        }
    }
}


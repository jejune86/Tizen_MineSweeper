using System;
using Tizen.NUI;

namespace MineSweeper.Services
{
    /// <summary>
    /// 게임 타이머를 관리하는 서비스 클래스
    /// </summary>
    public class GameTimerService : IDisposable
    {
        private Timer gameTimer;
        private int elapsedTime;
        private bool isRunning;

        public event Action<int> TimeUpdated;

        public int ElapsedTime
        {
            get => elapsedTime;
            private set
            {
                if (elapsedTime != value)
                {
                    elapsedTime = value;
                    TimeUpdated?.Invoke(elapsedTime);
                }
            }
        }

        public bool IsRunning => isRunning;

        /// <summary>
        /// 타이머를 시작합니다.
        /// </summary>
        public void Start()
        {
            Stop();
            isRunning = true;
            gameTimer = new Timer(1000); // 1초마다 실행
            gameTimer.Tick += OnTimerTick;
            gameTimer.Start();
        }

        /// <summary>
        /// 타이머를 중지합니다.
        /// </summary>
        public void Stop()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Tick -= OnTimerTick;
                gameTimer.Dispose();
                gameTimer = null;
            }
            isRunning = false;
        }

        /// <summary>
        /// 타이머를 리셋합니다.
        /// </summary>
        public void Reset()
        {
            Stop();
            ElapsedTime = 0;
        }

        private bool OnTimerTick(object sender, Timer.TickEventArgs e)
        {
            ElapsedTime++;
            return true;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}


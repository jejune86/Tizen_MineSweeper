using System;
using Tizen.NUI;

namespace MineSweeper.Services
{
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

        public void Start()
        {
            Stop();
            isRunning = true;
            gameTimer = new Timer(1000); // 1초마다 실행
            gameTimer.Tick += OnTimerTick;
            gameTimer.Start();
        }

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


using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class PerformanceManager : SingletonMonoSimple<PerformanceManager>
    {
        public const int CriticalFps = 10;

        public int AverageFps { get; private set; }

        public int CurrentFps { get; private set; }

        private float _averageFpsCounter;

        private int _averageCount;

        public bool IsFpsCritical => AverageFps <= CriticalFps;

        protected override void OnAwaken()
        {
            Update();
        }


        private void Update()
        {
            CurrentFps = (int)(1f / Time.unscaledDeltaTime);

            _averageFpsCounter += CurrentFps;

            _averageCount++;

            if (_averageCount <= 30)
                return;

            AverageFps = (int)(_averageFpsCounter / _averageCount);

            _averageFpsCounter = 0;
            _averageCount = 0;
        }
    }
}

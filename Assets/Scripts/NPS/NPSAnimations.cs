using UHFPS.Runtime;
using UnityEngine;
using Zenject;

namespace ReaperGS
{
    public class NPSAnimations : MonoBehaviour
    {
        private SoundManager _soundManager;
        [SerializeField] private SoundClip[] _stepsClip;

        private int _currentStepIndex = 0;

        [Inject]
        private void Construct(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public void PerformStep()
        {
            _soundManager.PlaySound(_stepsClip[_currentStepIndex], transform);

            _currentStepIndex++;
            if (_currentStepIndex >= _stepsClip.Length)
            {
                _currentStepIndex = 0;
            }
        }
    }
}

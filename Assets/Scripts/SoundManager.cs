using cherrydev;
using UHFPS.Runtime;
using UHFPS.Tools;
using UnityEngine;
using Zenject;

namespace ReaperGS
{
    public class SoundManager : MonoBehaviour
    {
        private DialogBehaviour _dialogueBehaiviour;

        [SerializeField] private SoundClip[] _dialogueTapClips;
        private int _currentIndex = 0;

        [SerializeField] private SoundClip _suspenceClip;
        [SerializeField] private SoundClip _endDemoClip;

        [Inject]
        private void Construct(DialogBehaviour dialogBehaviour)
        {
            _dialogueBehaiviour = dialogBehaviour;
        }

        private void OnEnable()
        {
            _dialogueBehaiviour.OnDialogTextCharWrote += PlayTapSound;
        }

        private void OnDisable()
        {
            _dialogueBehaiviour.OnDialogTextCharWrote -= PlayTapSound;
        }

        private void PlayTapSound()
        {
            PlaySound2D(_dialogueTapClips[_currentIndex]);

            _currentIndex++;
            if (_currentIndex >= _dialogueTapClips.Length)
            {
                _currentIndex = 0;
            }
        }


        public void PlaySound(SoundClip soundClip, Transform position)
        {
            GameTools.PlayOneShot3D(position.position, soundClip);
        }



        public void PlaySound2D(SoundClip soundClip)
        {
            GameTools.PlayOneShot2D(transform.position, soundClip, "2D Sound");
        }

        public void TriggerSuspence()
        {
            PlaySound2D(_suspenceClip);
        }

        public void TriggerDemoEnd()
        {
            PlaySound2D(_endDemoClip);
        }
    }
}

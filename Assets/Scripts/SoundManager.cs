using UHFPS.Runtime;
using UHFPS.Tools;
using UnityEngine;

namespace ReaperGS
{
    public class SoundManager : MonoBehaviour
    {
        //private SoundTrigger[] _soundTriggers;
        //
        //private void Awake()
        //{
        //    _soundTriggers = new SoundTrigger[transform.childCount];
        //    for (int i = 0; i < _soundTriggers.Length; i++)
        //    {
        //        _soundTriggers[i] = transform.GetChild(i).GetComponent<SoundTrigger>();
        //    }
        //}

        public void PlaySound(SoundClip soundClip, Transform position)
        {
            //for (int i = 0; i < _soundTriggers.Length; i++)
            //{
            //    SoundTrigger soundTrigger = _soundTriggers[i];
            //    if (soundTrigger.TriggerSound == soundClip)
            //    {
            //        soundTrigger.PlaySound();
            //    }
            //}
            GameTools.PlayOneShot3D(position.position, soundClip);
        }

        public void PlaySound2D(SoundClip soundClip)
        {
            GameTools.PlayOneShot2D(transform.position, soundClip, "2D Sound");
        }
    }
}

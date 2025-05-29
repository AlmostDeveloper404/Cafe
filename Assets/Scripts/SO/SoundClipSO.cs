using UnityEngine;

namespace ReaperGS
{
    [CreateAssetMenu(menuName = "ReaperGS/NewSoundClip")]
    public class SoundClipSO : ScriptableObject
    {
        public AudioClip audioClip;
        public float volume = 1f;
    }
}

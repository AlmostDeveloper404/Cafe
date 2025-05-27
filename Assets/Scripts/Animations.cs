using UnityEngine;

namespace ReaperGS
{
    public enum GetLengthAnimationStates
    {
        CoffeeCup
    }

    public static class Animations
    {
        public static readonly int CoffeeCup = Animator.StringToHash(GetLengthAnimationStates.CoffeeCup.ToString());


        public static float GetAnimationTime(Animator animator, string animationName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == animationName)
                {

                    return clip.length;
                }
            }
            return default(float);
        }

    }
}

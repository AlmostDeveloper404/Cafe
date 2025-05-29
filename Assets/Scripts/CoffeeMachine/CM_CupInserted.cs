using UHFPS.Runtime;
using UnityEngine;

namespace ReaperGS
{
    public class CM_CupInserted : CM_Base<CoffeeMachine>
    {
        private ParticleSystem _coffeeStreamParticles;
        private Animator _coffeeCupAnimator;
        private SoundManager _soundManager;
        private SoundClip _soundClip;

        public CM_CupInserted(Animator animator, ParticleSystem coffeeStreamParticles, SoundManager soundManager, SoundClip soundClip)
        {
            _coffeeCupAnimator = animator;
            _coffeeStreamParticles = coffeeStreamParticles;
            _soundManager = soundManager;
            _soundClip = soundClip;
        }

        public override void EntryState(CoffeeMachine coffeeMachine)
        {
            _coffeeCupAnimator.SetTrigger(Animations.CoffeeCup);

            _soundManager.PlaySound(_soundClip, coffeeMachine.transform);
            _coffeeStreamParticles.Play();


            float timeToWait = Animations.GetAnimationTime(_coffeeCupAnimator, GetLengthAnimationStates.CoffeeCup.ToString());

            coffeeMachine.WaitForSecondsWithAction(timeToWait, coffeeMachine.ChangeToAwaitLidState);
        }

        public override void ExitState(CoffeeMachine coffeeMachine)
        {


            _coffeeStreamParticles.Stop();
        }

        public override void UpdateState(CoffeeMachine coffeeMachine)
        {

        }
    }
}

using UnityEngine;

namespace ReaperGS
{
    public class CM_CupInserted : CM_Base<CoffeeMachine>
    {
        private ParticleSystem _coffeeStreamParticles;
        private Animator _coffeeCupAnimator;

        public CM_CupInserted(Animator animator, ParticleSystem coffeeStreamParticles)
        {
            _coffeeCupAnimator = animator;
            _coffeeStreamParticles = coffeeStreamParticles;
        }

        public override void EntryState(CoffeeMachine coffeeMachine)
        {
            _coffeeCupAnimator.SetTrigger(Animations.CoffeeCup);
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

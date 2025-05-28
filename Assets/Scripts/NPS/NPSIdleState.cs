using UnityEngine;
using UnityEngine.AI;

namespace ReaperGS
{
    public class NPSIdleState : NPSBase<NPS>
    {
        private Animator _animator;

        public NPSIdleState(Animator animator)
        {
            _animator = animator;
        }

        public override void EntryState(NPS nps)
        {
            _animator.SetBool(Animations.IsWalking, false);
        }

        public override void ExitState(NPS nps)
        {
            
        }

        public override void UpdateState(NPS nps)
        {
            
        }
    }
}

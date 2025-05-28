using cherrydev;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;

namespace ReaperGS
{
    public class NPSDialogueState : NPSBase<NPS>
    {
        private DialogBehaviour _dialogueBehaivior;
        private Animator _animator;

        public NPSDialogueState(Animator animator, DialogBehaviour dialogBehaviour)
        {
            _animator = animator;
            _dialogueBehaivior = dialogBehaviour;
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

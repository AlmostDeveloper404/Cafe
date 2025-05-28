using UnityEngine;
using UnityEngine.AI;
using System;

namespace ReaperGS
{
    public class NPSGoToPointState : NPSBase<NPS>
    {
        private NavMeshAgent _navMesh;
        private Animator _animator;
        private Transform _targetToReach;
        private float _speed;

        //private Action _actionAfterReachingTarget;


        public NPSGoToPointState(Animator animator, NavMeshAgent navMeshAgent, Transform target, float speed)
        {
            _animator = animator;
            _navMesh = navMeshAgent;
            _targetToReach = target;
            _speed = speed;
        }

        public override void EntryState(NPS nps)
        {
            _navMesh.speed = _speed;
            _animator.SetBool(Animations.IsWalking, true);
            _navMesh.isStopped = false;
            _navMesh.SetDestination(_targetToReach.position);
        }

        public override void ExitState(NPS nps)
        {
            nps.TurnNPSToTarget(_targetToReach, true, null);
        }

        public override void UpdateState(NPS nps)
        {
            Vector3 distanceToPoint = _targetToReach.position - nps.transform.position;
            if (distanceToPoint.magnitude < 0.2f)
            {
                //_actionAfterReachingTarget?.Invoke();
                _navMesh.isStopped = true;
                nps.ChangeToIdle();
            }
        }
    }
}

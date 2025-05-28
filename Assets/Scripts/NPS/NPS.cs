using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using System.Collections;
using cherrydev;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System;

namespace ReaperGS
{
    public class NPS : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _turnHeadCurve;
        [SerializeField] private float _rotateHeadDuration = 1f;
        [SerializeField] private float _walkSpeed = 2f;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private LookAtIK _lookAtIK;
        private AimIK _aimIK;

        private NPSBase<NPS> _currentState;
        private NPSGoToPointState _goToPointState;
        private NPSIdleState _npsIdleState;
        private NPSDialogueState _npsDialogueState;

        private DialogBehaviour _dialogueBehavior;
        private FPSCameraController _fpsCameraController;

        private Coroutine _headTurnCoroutine;
        private Coroutine _turnCoroutine;

        [SerializeField] private Transform _trackHeadTarget;

        [Inject]
        private void Construct(DialogBehaviour dialogBehaviour,FPSCameraController fPSCameraController)
        {
            _dialogueBehavior = dialogBehaviour;
            _fpsCameraController = fPSCameraController;
        }

        private void Awake()
        {
            _aimIK = GetComponentInChildren<AimIK>();
            _lookAtIK = GetComponentInChildren<LookAtIK>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _npsIdleState = new NPSIdleState(_animator);
            ChangeState(_npsIdleState);
        }

        private void Update()
        {
            _currentState?.UpdateState(this);
        }

        private void LateUpdate()
        {
            _lookAtIK.solver.Update();
        }

        public void TriggerAnimationByHash(int hash)
        {
            _animator.SetTrigger(hash);
        }

        public void ChangeState(NPSBase<NPS> newState)
        {
            _currentState?.ExitState(this);
            _currentState = newState;
            _currentState?.EntryState(this);
        }

        public void PlaceNPCToPoint(Transform point)
        {
            //_navMeshAgent.enabled = false;
            transform.position = point.position;
            transform.rotation = point.rotation;
            //_navMeshAgent.enabled = true;
        }


        public void ToggleNavMesh(bool isEnabled)
        {
            _navMeshAgent.enabled = isEnabled;
        }

        public void GoToTarget(Transform target)
        {
            _goToPointState = new NPSGoToPointState(_animator, _navMeshAgent, target, _walkSpeed);
            ChangeState(_goToPointState);
        }

        private void DialogueStarted()
        {
            SetLookIKTargetToDefault();
            ChangeState(_npsDialogueState);
        }

        private void DialogueFinished()
        {
            StopLooking();
            ChangeToIdle();
        }

        private void SetLookIKTargetToDefault()
        {
            _lookAtIK.solver.IKPosition = transform.position + transform.forward + Vector3.up * 2f;
        }

        public void ChangeToIdle()
        {
            ChangeState(_npsIdleState);
        }

        public void TurnToPlayer()
        {
            TurnNPSToTarget(_fpsCameraController.transform, false, null);
        }

        public void TurnNPSToTargetWithAction(Transform target, bool isTargetForward, Action action)
        {
            TurnNPSToTarget(target, isTargetForward, action);
        }

        public void TurnNPSToTarget(Transform target, bool isTargetForward, Action action)
        {
            if (_turnCoroutine != null)
            {
                StopCoroutine(_turnCoroutine);
            }

            _turnCoroutine = StartCoroutine(Turn(target, isTargetForward, action));
        }

        private IEnumerator Turn(Transform target, bool isTargetForward, Action action)
        {
            _navMeshAgent.enabled = false;

            Quaternion startRotation = transform.rotation;

            Vector3 directionToTarget = isTargetForward ? target.forward : target.position - transform.position;
            directionToTarget.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            Vector3 cross = Vector3.Cross(transform.forward, directionToTarget);
            _animator.SetTrigger(cross.y > 0 ? Animations.TurnRight : Animations.TurnLeft);

            float elapsedTime = 0f;
            float duration = 1f;

            while (elapsedTime < duration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            action?.Invoke();

            transform.rotation = targetRotation;
            _navMeshAgent.enabled = true;
        }

        public void StartLookingAt(Transform target)
        {
            if (_headTurnCoroutine != null)
            {
                StopCoroutine(_headTurnCoroutine);
            }

            _headTurnCoroutine = StartCoroutine(TurnHeadToTarget(target, 1f));
        }

        public void StopLooking()
        {
            if (_headTurnCoroutine != null)
            {
                StopCoroutine(_headTurnCoroutine);
            }

            _headTurnCoroutine = StartCoroutine(TurnHeadToTarget(null, 0f));
        }

        private IEnumerator TurnHeadToTarget(Transform target, float targetWeight)
        {
            float initialWeight = _lookAtIK.solver.IKPositionWeight;
            float time = 0f;

            Vector3 initialPosition = _lookAtIK.solver.IKPosition;
            _trackHeadTarget.position = _lookAtIK.solver.IKPosition;
            Vector3 targetPosition = target != null ? target.position : transform.position + transform.forward;

            while (time < _rotateHeadDuration)
            {
                time += Time.deltaTime;
                float t = _turnHeadCurve.Evaluate(time / _rotateHeadDuration);

                _lookAtIK.solver.IKPositionWeight = Mathf.Lerp(initialWeight, targetWeight, t);
                _lookAtIK.solver.IKPosition = Vector3.Lerp(initialPosition, targetPosition, t);
                _trackHeadTarget.position = _lookAtIK.solver.IKPosition;

                yield return null;
            }

            _lookAtIK.solver.IKPositionWeight = targetWeight;
            _lookAtIK.solver.IKPosition = targetPosition;
            _trackHeadTarget.position = _lookAtIK.solver.IKPosition;
        }
    }
}

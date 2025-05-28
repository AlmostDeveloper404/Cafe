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
using Unity.Properties;

namespace ReaperGS
{
    public class NPS : MonoBehaviour
    {
        [SerializeField] private DialogNodeGraph _startDialogue;
        [SerializeField] private DialogNodeGraph _dialogueFinished;

        [SerializeField] private AnimationCurve _turnHeadCurve;
        [SerializeField] private float _rotateHeadDuration = 1f;
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private Transform _walkToPlayerPoint;
        [SerializeField] private Transform _lastWalkPoint;
        [SerializeField] private Transform _targetsHolder;
        [SerializeField] private Transform _screamerStandPoint;

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
        private GameManager _gameManager;
        private UIManager _uiManager;

        private Coroutine _headTurnCoroutine;
        private Coroutine _turnCoroutine;

        [field: SerializeField] public Transform LookAtTargetForPlayer;

        [SerializeField] private Transform _trackHeadTarget;

        [Inject]
        private void Construct(DialogBehaviour dialogBehaviour, FPSCameraController fPSCameraController, GameManager gameManager, UIManager uIManager)
        {
            _dialogueBehavior = dialogBehaviour;
            _fpsCameraController = fPSCameraController;
            _gameManager = gameManager;
            _uiManager = uIManager;
        }

        private void Awake()
        {
            _aimIK = GetComponentInChildren<AimIK>();
            _lookAtIK = GetComponentInChildren<LookAtIK>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            _gameManager.OnNewGameStateEntered += HandleGameStates;

            _dialogueBehavior.OnDialogueStarted += DialogueStarted;
            _dialogueBehavior.OnDialogueFinished += DialogueFinished;
        }

        private void OnDisable()
        {
            _gameManager.OnNewGameStateEntered -= HandleGameStates;

            _dialogueBehavior.OnDialogueStarted -= DialogueStarted;
            _dialogueBehavior.OnDialogueFinished -= DialogueFinished;
        }

        private void Start()
        {
            _targetsHolder.parent = null;

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

        private void HandleGameStates(GameStates gameStates)
        {
            switch (gameStates)
            {
                case GameStates.WaitForPlayerInput:
                    break;
                case GameStates.DemoStarted:
                    GoToTarget(_walkToPlayerPoint, () => _dialogueBehavior.StartDialog(_startDialogue));
                    break;
                case GameStates.FadingIntoCutscene:
                    GoToTarget(_lastWalkPoint, null);
                    break;
                case GameStates.LastCutsceneStarted:
                    PlaceNPCToPoint(_screamerStandPoint);
                    break;
                default:
                    break;
            }
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

        public void GoToTarget(Transform target, Action action)
        {
            _goToPointState = new NPSGoToPointState(_animator, _navMeshAgent, target, _walkSpeed, action);
            ChangeState(_goToPointState);
        }

        private void DialogueStarted()
        {
            ChangeState(_npsDialogueState);
        }

        private void DialogueFinished()
        {
            StopLooking();
            ChangeToIdle();
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

        private void OnCollisionEnter(Collision collision)
        {

            CoffeeCup coffeeCap = collision.collider.GetComponentInParent<CoffeeCup>();
            if (coffeeCap)
            {
                coffeeCap.ReturnToPool();
                _uiManager.ShowMoneyEarnedUI(() => _dialogueBehavior.StartDialog(_dialogueFinished));
                _dialogueBehavior.OnDialogueFinished += TriggerFadingToCutscene;
                TurnToPlayer();
            }
        }

        private void TriggerFadingToCutscene()
        {
            _dialogueBehavior.OnDialogueFinished -= TriggerFadingToCutscene;
            _gameManager.ChangeGameState(GameStates.FadingIntoCutscene);
        }
    }
}

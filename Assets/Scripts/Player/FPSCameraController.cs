using UnityEngine;
using System.Collections;
using Zenject;
using System;
using Unity.Cinemachine;
using System.Collections.Generic;
using cherrydev;
using UnityEngine.Playables;

namespace ReaperGS
{
    public class FPSCameraController : MonoBehaviour
    {


        public event Action OnCameraUpdated;
        public event Action OnCameraRotationUpdated;

        private PlayerInput _playerInput;
        private GameManager _gameManager;
        private DialogBehaviour _dialogueBehaiviour;

        [Header("Sensitivity")]
        public float rotationSensitivity = 3f;
        public float yMinLimit = -89f;
        public float yMaxLimit = 89f;
        public float xCarMaxLimit = 60f;
        public float xCarMinLimit = -60f;

        private Vector2 _lookInput;
        private float _xRotation, _yRotation;

        [SerializeField] private float _slerpSpeed;
        [SerializeField] private float _smoothTime;

        private Vector3 _currentLookDirection;
        private Vector3 _lookHandleCameraVelocity;

        [Header("Zoom")]
        [SerializeField] private float _defaultZoom;
        [SerializeField] private float _dialogueZoom;

        [SerializeField] private float _zoomTime;
        [SerializeField] private AnimationCurve _zoomCurve;

        [SerializeField] private Transform _cutsceneTarget;
        [SerializeField] private PlayableDirector _playableDirector;

        private Coroutine _zoomCoroutine;
        private bool _isControlFreezed = false;

        private CinemachineCamera _camera;

        [Header("Target References")]
        [SerializeField] private Transform _cameraHolder;
        [SerializeField] private Transform _cameraLookAtPoint;
        [SerializeField] private Transform _lookForwardTarget;

        private Transform _currentTarget;
        private Vector3 _target;

        [SerializeField] private NPS _nps;


        [Inject]
        private void Construct(PlayerInput playerInput, GameManager gameManager, DialogBehaviour dialogBehaviour)
        {
            _playerInput = playerInput;
            _gameManager = gameManager;
            _dialogueBehaiviour = dialogBehaviour;
        }

        private void Awake()
        {
            _camera = GetComponentInChildren<CinemachineCamera>();
        }

        private void OnEnable()
        {
            _playerInput.OnLook += ReadLookInput;
            _gameManager.OnNewGameStateEntered += HandleGameStates;

            _dialogueBehaiviour.OnDialogueStarted += ZoomIn;
            _dialogueBehaiviour.OnDialogueFinished += ZoomOut;

            _dialogueBehaiviour.OnSentenceNodeActive += LockCursor;
            _dialogueBehaiviour.OnAnswerNodeActive += UnlockCursor;
        }

        private void OnDisable()
        {
            _playerInput.OnLook -= ReadLookInput;
            _gameManager.OnNewGameStateEntered -= HandleGameStates;

            _dialogueBehaiviour.OnDialogueStarted -= ZoomIn;
            _dialogueBehaiviour.OnDialogueFinished -= ZoomOut;

            _dialogueBehaiviour.OnSentenceNodeActive -= LockCursor;
            _dialogueBehaiviour.OnAnswerNodeActive -= UnlockCursor;
        }

        private void Start()
        {
            SetNewTarget(_nps.LookAtTargetForPlayer);

            Vector3 angles = _camera.transform.eulerAngles;
            _xRotation = angles.y;
            _yRotation = angles.x;

            LockCursor();
            GameStarted();
        }

        private void LateUpdate()
        {

            if (!_currentTarget)
            {
                _target = _camera.transform.position + _camera.transform.forward * 10f;
                _cameraLookAtPoint.position = _target;
                HandleCamera(true);
            }
            else
            {
                //_cameraLookAtPoint.position = Vector3.SmoothDamp(_cameraLookAtPoint.position, _currentTarget.position, ref _lookToTargetVelocity, _smoothTime);
                _cameraLookAtPoint.position = Vector3.Lerp(_cameraLookAtPoint.position, _currentTarget.position, _slerpSpeed * Time.deltaTime);
                _cameraLookAtPoint.rotation = Quaternion.Slerp(_cameraLookAtPoint.rotation, _currentTarget.rotation, _slerpSpeed * Time.deltaTime);
                RotateTowardsPoint(_cameraLookAtPoint);
            }
            OnCameraUpdated?.Invoke();
            OnCameraRotationUpdated?.Invoke();
        }

        private void HandleGameStates(GameStates gameStates)
        {
            switch (gameStates)
            {
                case GameStates.WaitForPlayerInput:
                    StopCameraControl();
                    break;
                case GameStates.LastCutsceneStarted:
                    StopCameraControl();
                    SetNewTarget(_cutsceneTarget);
                    _playableDirector.Play();
                    break;
                default:
                    break;
            }
        }

        private void GameStarted()
        {
            _camera.transform.parent = null;
            _cameraLookAtPoint.parent = null;
            StartFollowTarget(_cameraHolder);
        }

        public void SetDefaultTarget()
        {
            SetNewTarget(_lookForwardTarget);
        }

        public void SetNewTarget(Transform target)
        {
            Vector3 currentLookDirection = _cameraLookAtPoint.position - _camera.transform.position;
            SetCurrentLookDirection(currentLookDirection);
            _currentTarget = target;
        }

        public void ChangeClippingPlance(float value)
        {
            _camera.Lens.NearClipPlane = value;
        }

        public void StartFollowTarget(Transform cameraHolder)
        {
            _camera.Target.TrackingTarget = cameraHolder;
        }

        private float NormalizeAngle(float angle)
        {
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }

        public void SetRotation()
        {
            Vector3 localEuler = _camera.transform.localEulerAngles;
            _xRotation = NormalizeAngle(localEuler.y);
            _yRotation = NormalizeAngle(localEuler.x);
        }

        private void ReadLookInput(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }

        public void RotateTowardsPoint(Transform point)
        {
            _currentLookDirection = Vector3.SmoothDamp(_currentLookDirection, point.position - _camera.transform.position, ref _lookHandleCameraVelocity, _smoothTime);

            if (_currentLookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_currentLookDirection.normalized);
                _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, targetRotation, _slerpSpeed * Time.deltaTime);
            }
        }

        public void SetCurrentLookDirection(Vector3 currentLookDirection)
        {
            _currentLookDirection = currentLookDirection;
        }

        public void StopCameraControl()
        {
            _isControlFreezed = true;
        }

        public void ResumeCameraControl()
        {
            _currentTarget = null;
            _isControlFreezed = false;
            SetRotation();
        }

        private void LockCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void UnlockCursor(AnswerNode answerNode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HandleCamera(bool isFreeLookCamera)
        {
            if (_isControlFreezed) return;

            _xRotation += _lookInput.x * rotationSensitivity;
            _yRotation = ClampAngle(_yRotation - _lookInput.y * rotationSensitivity, yMinLimit, yMaxLimit);

            if (!isFreeLookCamera)
            {
                _xRotation = ClampAngle(_xRotation, xCarMinLimit, xCarMaxLimit);
            }

            _camera.transform.localRotation = Quaternion.AngleAxis(_xRotation, Vector3.up) * Quaternion.AngleAxis(_yRotation, Vector3.right);
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        private void ZoomIn()
        {
            SetNewTarget(_nps.LookAtTargetForPlayer);
            StopCameraControl();

            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }

            _zoomCoroutine = StartCoroutine(ZoomTo(_dialogueZoom));
        }

        private void ZoomOut()
        {
            ResumeCameraControl();

            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }
            _zoomCoroutine = StartCoroutine(ZoomTo(_defaultZoom));
        }

        public void Zoom(float amount)
        {
            if (_zoomCoroutine != null)
            {
                StopCoroutine(_zoomCoroutine);
            }
            _zoomCoroutine = StartCoroutine(ZoomTo(amount));
        }

        private IEnumerator ZoomTo(float targetFOV)
        {
            float startFOV = _camera.Lens.FieldOfView;
            float elapsed = 0f;

            while (elapsed < _zoomTime)
            {
                elapsed += Time.deltaTime;
                float t = _zoomCurve.Evaluate(elapsed / _zoomTime);
                _camera.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
                yield return null;
            }

            _camera.Lens.FieldOfView = targetFOV;
        }


    }
}

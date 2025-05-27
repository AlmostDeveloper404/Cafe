using UnityEngine;
using Zenject;
using System;
using System.Security.Cryptography;
using System.Collections;

namespace ReaperGS
{
    public enum MovementState
    {
        Idle, Walking, Running
    }

    public class PlayerMovement : MonoBehaviour
    {
        #region HandleInput
        private Vector2 _movementInput;
        private bool _isRunningInput;
        #endregion

        [Header("Transform References")]
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Transform _orientation;
        [SerializeField] private Transform _cameraTransform;

        [Header("Movement Settings")]
        [SerializeField, Range(0f, 180f)] private float _maxTurnAngle = 80f;
        private float _moveSpeed;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;
        private bool _isWalking;
        private bool _isRunning;
        private bool _isCrouching;

        private CharacterController _controller;

        private PlayerInput _playerInput;
        private FPSCameraController _fpsCameraController;

        public event Action<MovementState> OnStateChanged;
        public event Action OnCarLeft;

        private MovementState _currentState;

        [SerializeField] private float _gravity = 9.81f;
        private Vector3 _velocity;

        private bool _isMovementFreezed = false;
        private bool _isRotationFreezed = false;

        [Inject]
        private void Construct(PlayerInput playerInput, FPSCameraController fpsCameraController)
        {
            _playerInput = playerInput;
            _fpsCameraController = fpsCameraController;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _playerInput.OnMove += ReadMoveInput;
            _playerInput.OnRun += ReadRunInput;

            _fpsCameraController.OnCameraRotationUpdated += RotateCharacter;
        }

        private void OnDisable()
        {
            _playerInput.OnMove -= ReadMoveInput;
            _playerInput.OnRun -= ReadRunInput;

            _fpsCameraController.OnCameraRotationUpdated -= RotateCharacter;
        }

        private void Update()
        {
            if (_isMovementFreezed) return;

            HandleWalking();
            HandleSprint();
            UpdateCurrentMovementState();
            MovePlayer();
        }

        public void LeaveCar()
        {
            transform.parent = null;
            OnCarLeft?.Invoke();
        }

        public void SetPosition(Transform point)
        {
            _controller.enabled = false;
            transform.rotation = Quaternion.identity;
            transform.position = point.position;
            _orientation.rotation = point.rotation;
            _controller.enabled = true;
        }

        private void ReadMoveInput(Vector2 movementInput)
        {
            _movementInput = _isMovementFreezed ? Vector2.zero : movementInput;
        }

        private void ReadRunInput(bool isRunningInput)
        {
            _isRunningInput = isRunningInput;
        }
        private void RotateCharacter()
        {
            if (_isRotationFreezed) return;

            if (_maxTurnAngle >= 180f) return;

            Quaternion cameraRotation = Quaternion.LookRotation(new Vector3(_cameraTransform.transform.forward.x, 0f, _cameraTransform.transform.forward.z));
            _orientation.rotation = cameraRotation;


            if (_maxTurnAngle <= 0f)
            {
                _bodyTransform.localRotation = cameraRotation;
                return;
            }

            Vector3 camRelative = _bodyTransform.InverseTransformDirection(_cameraTransform.transform.forward);

            float angle = Mathf.Atan2(camRelative.x, camRelative.z) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) > Mathf.Abs(_maxTurnAngle))
            {
                float a = angle - _maxTurnAngle;
                if (angle < 0f) a = angle + _maxTurnAngle;


                _bodyTransform.rotation = Quaternion.AngleAxis(a, _bodyTransform.up) * _bodyTransform.rotation;
            }
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = _orientation.forward * _movementInput.y + _orientation.right * _movementInput.x;
            moveDirection.y = 0;

            Vector3 move = moveDirection * _moveSpeed;

            if (!_controller.isGrounded)
            {
                _velocity.y -= _gravity * Time.deltaTime;
            }
            else
            {
                _velocity.y = -1f;
            }

            _controller.Move((move + _velocity) * Time.deltaTime);
        }


        private void HandleWalking()
        {
            bool isMoving = _movementInput != Vector2.zero;
            _isWalking = isMoving && !_isRunning && !_isCrouching;
        }

        private void HandleSprint()
        {
            bool isMovingForward = _movementInput.y > 0;
            bool isMovingSidewaysWhileForward = _movementInput.y > 0 && _movementInput.x != 0;

            _isRunning = (_isRunningInput && (isMovingForward || isMovingSidewaysWhileForward));

            _moveSpeed = _isRunning
                ? _runSpeed
                : _walkSpeed;
        }



        private void UpdateCurrentMovementState()
        {
            MovementState newState;

            if (_isRunning)
                newState = MovementState.Running;
            else if (_isWalking)
                newState = MovementState.Walking;
            else
                newState = MovementState.Idle;

            if (_currentState != newState)
            {
                _currentState = newState;
                OnStateChanged?.Invoke(_currentState);
            }
        }

        public void FreezeMovement(bool isMovementFreezed)
        {
            _isMovementFreezed = isMovementFreezed;
            _isRunning = false;
            _isWalking = false;
            UpdateCurrentMovementState();
        }

        public void FreezeRotation(bool isRotationFreezed)
        {
            _isRotationFreezed = isRotationFreezed;
        }


        public void BlendToPoint(Transform point, Action action)
        {
            FreezeMovement(true);
            FreezeRotation(true);
            StartCoroutine(MoveToTargetCoroutine(point, action));
        }

        private IEnumerator MoveToTargetCoroutine(Transform targetPoint, Action action)
        {
            _controller.enabled = false;

            float positionThreshold = 0.05f;
            float rotationThreshold = 1f;

            while (true)
            {
                // Перемещение
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    2f * Time.deltaTime
                );

                // Поворот
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint.forward);
                _bodyTransform.rotation = Quaternion.Slerp(
                    _bodyTransform.rotation,
                    targetRotation,
                    5f * Time.deltaTime
                );

                // Проверка окончания
                bool reachedPosition = Vector3.Distance(transform.position, targetPoint.position) < positionThreshold;
                bool reachedRotation = Quaternion.Angle(_bodyTransform.rotation, targetRotation) < rotationThreshold;

                if (reachedPosition && reachedRotation)
                    break;

                yield return null;
            }

            _controller.enabled = true;
            transform.parent = targetPoint;
            action?.Invoke();
        }
    }
}


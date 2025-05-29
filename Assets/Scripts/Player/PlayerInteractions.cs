using System;
using UnityEngine;
using Zenject;
using Unity.Cinemachine;
using UHFPS.Runtime;

namespace ReaperGS
{
    public class PlayerInteractions : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private SoundManager _soundManager;

        private CinemachineCamera _cameraController;

        public event Action<IDragable> OnInteractableOver;
        public event Action OnInteractionReset;

        public event Action<IDragable> OnDraggableChanged;
        public event Action<IDragable> OnDraggableReleased;

        [SerializeField] private float _raycastDistance;
        [SerializeField] private float _holdDistanceForwardMultiplier;
        [SerializeField] private LayerMask _raycastLayerMask;

        private Ray _rayFromCam;

        private bool _isInteractionsFreezed = false;

        private IDragable _currentDraggable;
        public IDragable GetCurrentDraggable => _currentDraggable;

        private IDragable _lastHoveredDraggable;

        [Header("Sounds")]
        [SerializeField] private SoundClip _throwSound;
        [SerializeField] private SoundClip _pickUpSound;

        [Inject]
        private void Construct(PlayerInput playerInput, SoundManager soundManager)
        {
            _playerInput = playerInput;
            _soundManager = soundManager;
        }

        private void Awake()
        {
            _cameraController = GetComponentInChildren<CinemachineCamera>();
        }

        protected virtual void OnEnable()
        {
            _playerInput.OnInteract += TryInteract;
            _playerInput.OnDrop += TryRelease;
            _playerInput.OnLMBPressed += Drag;
            _playerInput.OnThrowing += TryThrow;
        }

        protected virtual void OnDisable()
        {
            _playerInput.OnInteract -= TryInteract;
            _playerInput.OnDrop -= TryRelease;
            _playerInput.OnLMBPressed -= Drag;
            _playerInput.OnThrowing -= TryThrow;
        }

        private void Update()
        {
            if (_isInteractionsFreezed) return;


            _rayFromCam = new Ray(_cameraController.transform.position, _cameraController.transform.forward);

            RaycastHit raycastHit = GetRaycastHit();

            if (raycastHit.collider != null)
            {
                IDragable interactable = raycastHit.collider.GetComponentInParent<IDragable>();

                if (interactable != null)
                {
                    if (_lastHoveredDraggable != null && _lastHoveredDraggable != interactable)
                    {
                        _lastHoveredDraggable.StopHightlighting();
                    }

                    _lastHoveredDraggable = interactable;
                    _lastHoveredDraggable.Highlight();
                    OnInteractableOver?.Invoke(interactable);
                }
                else
                {
                    if (_lastHoveredDraggable != null)
                    {
                        _lastHoveredDraggable.StopHightlighting();
                        _lastHoveredDraggable = null;
                    }
                    OnInteractionReset?.Invoke();
                }
            }
            else
            {
                if (_lastHoveredDraggable != null)
                {
                    _lastHoveredDraggable.StopHightlighting();
                    _lastHoveredDraggable = null;
                }
                OnInteractionReset?.Invoke();
            }
        }


        private void Drag()
        {
            if (_currentDraggable != null)
            {
                Vector3 dragTarget = _cameraController.transform.position + _cameraController.transform.forward * _holdDistanceForwardMultiplier;
                _currentDraggable.Drag(dragTarget);
            }
        }


        private void TryInteract()
        {
            if (_isInteractionsFreezed) return;

            RaycastHit raycastHit = GetRaycastHit();
            if (raycastHit.collider != null)
            {
                IDragable interactionObject = raycastHit.collider.GetComponentInParent<IDragable>();
                if (interactionObject != null)
                {
                    _soundManager.PlaySound2D(_pickUpSound);
                    Interact(interactionObject);
                }
            }
        }

        private void TryRelease()
        {
            if (_currentDraggable != null)
            {
                _currentDraggable.Release();
                UnfreezeInteractions();
                OnDraggableReleased?.Invoke(_currentDraggable);

                _currentDraggable = null;
                OnDraggableChanged?.Invoke(_currentDraggable);
            }
        }

        private void TryThrow()
        {
            if (_currentDraggable != null)
            {
                _soundManager.PlaySound2D(_throwSound);
                _currentDraggable.Throw();
                _currentDraggable = null;
                UnfreezeInteractions();
                OnDraggableChanged?.Invoke(_currentDraggable);
            }
        }

        private void Interact(IDragable draggable)
        {
            _currentDraggable = draggable;
            _currentDraggable.StartDragging();
            _currentDraggable.StopHightlighting();
            FreezeInteractions();
            OnDraggableChanged?.Invoke(_currentDraggable);
        }
        public void FreezeInteractions()
        {
            _isInteractionsFreezed = true;
            OnInteractionReset?.Invoke();
        }

        public void UnfreezeInteractions()
        {
            _isInteractionsFreezed = false;
        }

        private RaycastHit GetRaycastHit() => Physics.Raycast(_rayFromCam, out var hit, _raycastDistance, _raycastLayerMask) ? hit : default;
    }
}

using UnityEngine;
using System.Collections;
using System;

namespace ReaperGS
{
    public enum DraggableType
    {
        None, CoffeeCap, Lid
    }

    public class DraggableObject : MonoBehaviour, IDragable
    {
        private Rigidbody _rigidbody;
        private Collider _collider;
        private bool _isDragging = false;

        private float _dragSpeed = 15f;
        private Vector3 _targetPosition;

        [field: SerializeField] public DraggableSO DraggableSO;
        [SerializeField] private float _snapSpeedMultiplier = 0.5f;
        [SerializeField] private Transform _draggingPoint;

        public bool IsInSnapZone { get; private set; }

        private Coroutine _snappingCoroutine;
        private Outline _outline;

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            _outline = GetComponentInChildren<Outline>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _outline.enabled = false;
        }

        public string GetName() => DraggableSO.Name;
        public string GetInteractName() => DraggableSO.Action;

        public void StartDragging()
        {
            _isDragging = true;
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = 10f;
        }

        public void Drag(Vector3 targetPosition)
        {
            if (!_isDragging) return;

            _targetPosition = targetPosition;

            Vector3 direction = (_targetPosition - _draggingPoint.position);
            _rigidbody.linearVelocity = direction * _dragSpeed;
        }
        public void Throw()
        {
            Release();
            _rigidbody.AddForce(Camera.main.transform.forward * 5f, ForceMode.VelocityChange);
        }

        public void Release()
        {
            _isDragging = false;
            _rigidbody.useGravity = true;
            _rigidbody.linearDamping = 0f;
            _rigidbody.linearVelocity = Vector3.zero;
        }

        public void Highlight()
        {
            _outline.enabled = true;
        }

        public void StopHightlighting()
        {
            _outline.enabled = false;
        }

        public void SnapToPoint(Transform point)
        {
            if (_snappingCoroutine != null)
            {
                StopCoroutine(_snappingCoroutine);
            }

            _snappingCoroutine = StartCoroutine(PlaceToPointSmoothly(point, null));
        }

        public void SnapToPoint(Transform point, Action action)
        {
            if (_snappingCoroutine != null)
            {
                StopCoroutine(_snappingCoroutine);
            }

            _snappingCoroutine = StartCoroutine(PlaceToPointSmoothly(point, action));
        }

        private IEnumerator PlaceToPointSmoothly(Transform point, Action action)
        {
            _rigidbody.isKinematic = true;
            _collider.enabled = false;

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            for (float i = 0; i < 1f; i += Time.deltaTime * _snapSpeedMultiplier)
            {
                transform.position = Vector3.Lerp(startPos, point.position, i);
                transform.rotation = Quaternion.Lerp(startRot, point.rotation, i);
                yield return null;
            }

            transform.position = point.position;
            transform.rotation = point.rotation;
            action?.Invoke();

            gameObject.SetActive(false);
        }


        private void OnTriggerEnter(Collider other)
        {
            SnapZone snapZone = other.GetComponent<SnapZone>();
            if (snapZone)
            {
                IsInSnapZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            SnapZone snapZone = other.GetComponent<SnapZone>();
            if (snapZone)
            {
                IsInSnapZone = false;
            }
        }
    }
}

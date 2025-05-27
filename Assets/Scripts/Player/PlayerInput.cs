using UnityEngine;
using System;

namespace ReaperGS
{
    public class PlayerInput : MonoBehaviour
    {
        public event Action OnReload;
        public event Action<int> OnNumberPressed;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action<bool> OnRun;
        public event Action<bool> OnCrouch;
        public event Action OnJump;
        public event Action<KeyCode> OnKeyPressed;
        public event Action OnInteract;
        public event Action OnDrop;
        public event Action OnLMBPressed;
        public event Action OnThrowing;



        private Vector2 _previousMovement;


        private void Update()
        {
            ReadLMBHolding();
            ReadMoveInput();
            ReadLookInput();
            ReadRunInput();
            ReadJumpInput();
            ReadCrouchInput();
            ReadInteractInput();
            ReadDropInput();
            ReadNumberPressed();
            ReadReloadInput();
            ReadKeyInput();
            ReadRMBPressed();
        }

        private void ReadKeyInput()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    OnKeyPressed?.Invoke(key);
                    break;
                }
            }
        }

        private void ReadReloadInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnReload?.Invoke();
            }
        }

        private void ReadLMBHolding()
        {
            bool isPressing = Input.GetMouseButton(0);
            OnLMBPressed?.Invoke();
        }

        private void ReadRMBPressed()
        {
            if (Input.GetMouseButtonDown(1))
            {
                OnThrowing?.Invoke();
            }
        }

        private void ReadDropInput()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnDrop?.Invoke();
            }
        }

        private void ReadInteractInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnInteract?.Invoke();
            }
        }

        private void ReadMoveInput()
        {
            Vector2 moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveVector != _previousMovement)
            {
                OnMove?.Invoke(moveVector);
                _previousMovement = moveVector;
            }
        }

        private void ReadLookInput()
        {
            Vector2 look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            OnLook?.Invoke(look);
        }

        private void ReadRunInput()
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            OnRun?.Invoke(isRunning);
        }

        private void ReadJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnJump?.Invoke();
            }
        }

        private void ReadCrouchInput()
        {
            bool isCrouching = Input.GetKey(KeyCode.LeftControl);
            OnCrouch?.Invoke(isCrouching);
        }

        private void ReadNumberPressed()
        {
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + i)))
                {
                    OnNumberPressed?.Invoke(i);
                    break;
                }
            }
        }
    }
}

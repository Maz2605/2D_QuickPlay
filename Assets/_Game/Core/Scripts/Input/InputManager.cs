using System;
using _Script.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Game.Core.Scripts.Input
{
    public class InputManager : Singleton<InputManager>
    {
        public event Action<Vector2> OnTouchMove;
        public event Action<Vector2> OnTouchEnd;
        public event Action<Vector2> OnTouchStart; 
        
        private GameInput _inputActions; 
        private Camera _mainCamera;
        private bool _isDragging;

        protected override void Awake()
        {
            base.Awake();
            KeepAlive(true);
            _inputActions = new GameInput();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Touch.TouchContact.started += OnTouchPress;
            _inputActions.Touch.TouchContact.canceled += OnTouchCancel;
            _inputActions.Touch.TouchPosition.performed += OnTouchPosition;
        }

        public override void OnDisable()
        {
            _inputActions.Touch.TouchContact.started -= OnTouchPress;
            _inputActions.Touch.TouchContact.canceled -= OnTouchCancel;
            _inputActions.Touch.TouchPosition.performed -= OnTouchPosition;
            _inputActions.Disable();
        }

        private void OnTouchPress(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return; // Chặn UI
            _isDragging = true;

            OnTouchStart?.Invoke(_inputActions.Touch.TouchPosition.ReadValue<Vector2>());
        }

        private void OnTouchCancel(InputAction.CallbackContext ctx)
        {
            if (_isDragging)
            {
                _isDragging = false;
                OnTouchEnd?.Invoke(_inputActions.Touch.TouchPosition.ReadValue<Vector2>());
            }
        }

        private void OnTouchPosition(InputAction.CallbackContext ctx)
        {
            if (_isDragging)
            {
                OnTouchMove?.Invoke(ctx.ReadValue<Vector2>());
            }
        }

        public Vector3 GetWorldPosition()
        {
            Vector2 screenPos = _inputActions.Touch.TouchPosition.ReadValue<Vector2>();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            return worldPos;
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            if (UnityEngine.Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId);
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}

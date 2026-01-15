using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
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
        
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private PointerEventData _pointerEventData;

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
            Vector2 touchPos = _inputActions.Touch.TouchPosition.ReadValue<Vector2>();

            _pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = touchPos
            };
            _raycastResults.Clear();
            
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            bool isTouchingUI = _raycastResults.Count > 0;
            
            if (isTouchingUI)
            {
                foreach (var result in _raycastResults)
                {
                    // In ra tên object UI đang chặn chuột
                    // Dùng màu sắc để dễ nhìn trong Console
                    Debug.Log($"<color=yellow>Blocked by UI:</color> {result.gameObject.name}", result.gameObject);
                }
            }
            return isTouchingUI;
        }
    }
}

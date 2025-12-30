using System;
using UnityEngine;
using UnityEngine.InputSystem;
using _Script.DesignPattern.Singleton;

public class GameInputHandler : Singleton<GameInputHandler>
{
    public GameInput inputActions;

    public bool IsTouching { get; private set; }
    public bool WasReleasedThisFrame { get; private set; }
    public Vector2 TouchDelta { get; private set; }

    private Vector2 lastTouchPosition;
    private Vector2 currentTouchPosition;

    private bool releaseFlagQueued = false;
    protected override void Awake()
    {
        base.Awake();
        inputActions = new GameInput();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        inputActions.Touch.TouchContact.started += OnTouchStarted;
        inputActions.Touch.TouchContact.canceled += OnTouchEnded;
        inputActions.Touch.TouchPosition.performed += OnTouchMoved;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        inputActions.Touch.TouchContact.started -= OnTouchStarted;
        inputActions.Touch.TouchContact.canceled -= OnTouchEnded;
        inputActions.Touch.TouchPosition.performed -= OnTouchMoved;

        inputActions.Disable();
    }

    private void Update()
    {
        WasReleasedThisFrame = releaseFlagQueued;
        releaseFlagQueued = false;

        if (IsTouching)
        {
            TouchDelta = currentTouchPosition - lastTouchPosition;
            lastTouchPosition = currentTouchPosition;
        }
        else
        {
            TouchDelta = Vector2.zero;
        }
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        IsTouching = true;
        currentTouchPosition = lastTouchPosition = inputActions.Touch.TouchPosition.ReadValue<Vector2>();
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        IsTouching = false;
        releaseFlagQueued = true; 
        TouchDelta = Vector2.zero;
    }

    private void OnTouchMoved(InputAction.CallbackContext context)
    {
        currentTouchPosition = context.ReadValue<Vector2>();
    }
}

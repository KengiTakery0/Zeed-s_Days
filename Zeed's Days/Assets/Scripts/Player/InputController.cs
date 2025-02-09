using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public Vector2 MoveDirection { get; private set; }
    public Vector2 LookDirection { get; private set; }
    public bool isJump { get; private set; }
    public bool isInteract { get; private set; }
    public bool isShoot { get;  set; }
    public bool isSprint { get; private set; }
    public bool isAim { get; private set; }
    public bool OnTransport { get; set; }

    PlayerInputActions actions;
    PlayerInputActions.OnFootActions onFootActions;
    PlayerInputActions.OnTransportActions onTransportActions;

    [SerializeField] UnityEvent onAimEvent;
    [SerializeField] UnityEvent onJumpEvent;
    [SerializeField] UnityEvent onShootEvent;
    [SerializeField] UnityEvent onSprintEvent;
    [SerializeField] UnityEvent onInteractEvent;
    [SerializeField] UnityEvent onRelodEvent;
    

    private void Awake()
    {
        actions = new PlayerInputActions();
        onFootActions = actions.OnFoot;
        onTransportActions = actions.OnTransport;
        isAim = false;
        SetupInput();
    }
    
    public void SetupInput()
    {
        if (OnTransport) InitOntransport();
        else InitOnFoot();
    }
    private void OnEnable()
    {
        actions.Enable();     
    }
    private void OnDisable()
    {
        actions.Disable();
    }

    private void InitOntransport()
    {
        onTransportActions.Move.performed += ctx => OnTransportMove();
        onTransportActions.Turn.performed += ctx => OnTransportTurn();
    }
    private void OnTransportMove()
    {
        MoveDirection = new Vector2(0,onTransportActions.Move.ReadValue<float>());
    }
    private void OnTransportTurn()
    {
        MoveDirection = new Vector2(onTransportActions.Turn.ReadValue<float>(),0);
     
    }

    #region ONFOOT
    private void InitOnFoot()
    {
        onFootActions.Aim.performed += ctx => OnAim();
        onFootActions.Look.performed += ctx => OnLook();
        onFootActions.Jump.performed += ctx => OnJump();
        onFootActions.Shoot.performed += ctx => OnShoot();
        onFootActions.Move.performed += ctx => OnFootMove();
        onFootActions.Sprint.performed += ctx => OnSprint();
        onFootActions.Interact.performed += ctx => OnInteract();
        onFootActions.Reload.performed += ctx => OnReload();
    }

    private void OnReload()
    {
        onRelodEvent?.Invoke();
    }
    private void OnSprint()
    {
        isSprint = onFootActions.Sprint.IsPressed();
        onSprintEvent?.Invoke();
    }

    private void OnFootMove()
    {
        MoveDirection = onFootActions.Move.ReadValue<Vector2>();
    }
    private void OnLook()
    {
        LookDirection = onFootActions.Look.ReadValue<Vector2>();
    }
    private void OnJump()
    {
        isJump = onFootActions.Jump.IsPressed();
        onJumpEvent?.Invoke();
    }
    private void OnInteract()
    {
        isInteract = onFootActions.Interact.IsPressed();
        onInteractEvent?.Invoke();
    }
    private void OnShoot()
    {
        isShoot = onFootActions.Shoot.IsPressed();
        onShootEvent?.Invoke();
    }
    private void OnAim()
    {
        isAim = onFootActions.Aim.IsPressed();
        onAimEvent?.Invoke();
    }
    #endregion
}

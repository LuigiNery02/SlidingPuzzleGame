using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)] //input Reader needs to run first so everyone is aware of it when they initialize
public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }
    public Vector2 PointInput
    {
        get { return _playerInput.Gameplay.Point.ReadValue<Vector2>(); }
    }
    public event Action PressInputStarted;
    public event Action PressInputCanceled;

    private PlayerInput _playerInput;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInput = new PlayerInput();
    }
    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.Gameplay.Enable();

            _playerInput.Gameplay.Press.started += OnPressedStarted;
            _playerInput.Gameplay.Press.canceled += OnPressedCanceled;
        }
    }
    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.Gameplay.Disable();

            _playerInput.Gameplay.Press.started -= OnPressedStarted;
            _playerInput.Gameplay.Press.canceled -= OnPressedCanceled;
        }
    }
    private void OnPressedStarted(InputAction.CallbackContext context)
    {
        if(PressInputStarted != null)
        {
            PressInputStarted.Invoke();
        }
    }
    private void OnPressedCanceled(InputAction.CallbackContext context)
    {
        if (PressInputCanceled != null)
        {
            PressInputCanceled.Invoke();
        }
    }
}

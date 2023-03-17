using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Rendering.LookDev;

public class PlayerHandler : MonoBehaviour
{

    // Player Variable
    [SerializeField]
    private float _health = 10f;
    [Space]
    [SerializeField]
    private float _rotationalSpeed = 90f;
    [SerializeField]
    private float _speed = 5f;

    // Input System
    private PlayerInputs _inputActionAssets = new PlayerInputs();
    private InputAction _moveInput; // = _inputActionAssets.Gameplay.Move
    private InputAction _jumpInput; // = _inputActionAssets.Gameplay.Jump

    // Chaches
    [Space]
    [SerializeField]
    private Rigidbody _rigidbody; // = GetComponent<Rigidbody>()
    [SerializeField]
    private Animator _animator; // needs to be assigned manually

    private void Awake()
    {
        _inputActionAssets = new PlayerInputs();
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        _moveInput = _inputActionAssets.Gameplay.Move;
        _jumpInput = _inputActionAssets.Gameplay.Jump;
        _moveInput.Enable();
        _jumpInput.Enable();
    }

    private void OnDisable()
    {
        _moveInput.Disable();
        _jumpInput.Disable();
    }

    private void Update()
    {
        if (_moveInput != null && _moveInput.enabled && _moveInput.ReadValue<Vector2>() != Vector2.zero) Debug.Log(_moveInput.ReadValue<Vector2>());
    }

}

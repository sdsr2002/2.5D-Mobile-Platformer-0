using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;

public class PlayerHandler : MonoBehaviour
{
    ///
    /// Script Made by Kevin Håfström
    ///

    // Player Variable
    [Header("Player Variables")]
    [SerializeField]
    private float _health = 10f;
    [Space]
    [SerializeField]
    private float _rotationalSpeed = 90f;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _jumpHeight = 5f;
    [SerializeField]
    private float _mass = 5f;
    [SerializeField]
    private float _groundCheckDistance = 0.2f;

    private Vector3 _newDeltaMovePos;
    private Vector3 _currentGravity;

    // Input System
    [SerializeField]
    private PlayerInputsAsset _inputActionAssets;
    private InputAction _moveInput; // = _inputActionAssets.Gameplay.Move
    private InputAction _jumpInput; // = _inputActionAssets.Gameplay.Jump

    private Action<InputAction.CallbackContext> _jumpAction;
    private Coroutine _jumpingCoroutine;

    private bool _grounded => Physics.Raycast(Position, Vector3.down, _groundCheckDistance);

    // Chaches
    [Space]
    [Header("Chaches")]
    [SerializeField]
    private Rigidbody _rigidbody; // = GetComponent<Rigidbody>() | needs to be assigned manually if Rigidbody or CharacterController is not on GameObject
    [SerializeField]
    private CharacterController _characterController; // = GetComponent<Rigidbody>() | needs to be assigned manually if Rigidbody or CharacterController is not on GameObject
    private Transform transform 
    { 
        get
        {
            if (_characterController != null) return _characterController.transform;
            else return this.transform;
        }
    }

    [Header("Manually Assigned Chaches")]
    [SerializeField]
    private Animator _animator; // needs to be assigned manually

    public Vector3 Position {
        get { 
            if (_characterController != null) return _characterController.transform.position;
            else return transform.position;
        }
        private set 
        {
            if (_characterController != null)
            {
                _characterController.transform.position = value;
            }
            else 
            { 
                transform.position = value;
            } 
        } 
    }

    private void Awake()
    {
        _jumpAction = (ctx) => InputJump();
        _inputActionAssets = new PlayerInputsAsset();
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }
        GameManager.Instance.PlayerHandler = this;
    }

    private void OnEnable()
    {
        _moveInput = _inputActionAssets.Gameplay.Move;
        _jumpInput = _inputActionAssets.Gameplay.Jump;
        _moveInput.Enable();
        _jumpInput.performed += _jumpAction;
        _jumpInput.Enable();
    }

    private void OnDisable()
    {
        _moveInput.Disable();
        _jumpInput.performed -= _jumpAction;
        _jumpInput.Disable();
    }

    private void InputJump()
    {
        if (!_grounded) return;
        Jump();
    }

    private void Jump()
    {
        if (_jumpingCoroutine != null) StopCoroutine(_jumpingCoroutine);
        _jumpingCoroutine = StartCoroutine(JumpingCorutine());
    }

    private IEnumerator JumpingCorutine()
    {
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            _characterController.Move(Vector3.up * Time.deltaTime * _jumpHeight);
            _currentGravity = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    private void Update()
    {
        // Get Input
        GetInput();

        UpdateGravity();
        UpdateMovement();
        UpdateRotation();
        
    }

    private void GetInput()
    {
        _newDeltaMovePos = _moveInput.ReadValue<Vector2>();
    }

    private void UpdateGravity()
    {
        if (!_grounded)
        {
            _currentGravity += Physics.gravity * _mass * Time.deltaTime;
        }
        else if (_currentGravity != Vector3.zero)
        {
            _currentGravity = Vector3.zero;
        }
    }

    private void UpdateMovement()
    {
        _characterController.Move(Vector3.right * _newDeltaMovePos.x * _speed * Time.deltaTime + _currentGravity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.right * _newDeltaMovePos.x,Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Position,Position + Vector3.down * _groundCheckDistance);
    }

}

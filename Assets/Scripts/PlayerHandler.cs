using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    ///
    /// Script Made by Kevin Håfström
    ///

    #region PlayerVar
    // Player Variable
    [Header("--Player Variables--")]
    [SerializeField]
    private float _health = 10f;
    [Space]
    [SerializeField]
    private float _speed = 5f;
    private bool _isDead = false;
    #endregion

    #region JumpingVar
    [Header("-Jumping Variables")]
    [SerializeField]
    private float _jumpHeight = 5f;
    [SerializeField]
    private float _jumpDelay = 0f;
    [SerializeField]
    private float _jumpSpeed = 10f;
    [SerializeField]
    private float _JumpGroundCheckDistance = 0.5f;
    [SerializeField]
    private AnimationCurve _jumpCurve = AnimationCurve.EaseInOut(0,0,1,1);

    private float _jumpingFloat01 = 1f; //
    private float _jumpCorutineTimer = 0f;
    private Coroutine _jumpingCoroutine;

    #endregion

    #region GravityVar
    [Header("-Gravity Variables")]
    [SerializeField]
    private float _mass = 5f;
    [SerializeField]
    private float _groundCheckDistance = 0.2f;
    private Vector3 _currentGravity;
    #endregion

    #region InputVar
    private Vector3 _newDeltaMovePos;

    // Input System
    [SerializeField]
    private PlayerInputsAsset _inputActionAssets;
    private InputAction _moveInput; // = _inputActionAssets.Gameplay.Move
    private InputAction _jumpInput; // = _inputActionAssets.Gameplay.Jump

    private Action<InputAction.CallbackContext> _jumpAction;
    #endregion

    #region ChachesVar
    // Chaches
    [Space]
    [Header("Chaches")]
    [SerializeField]
    private Rigidbody _rigidbody; // = GetComponent<Rigidbody>() | needs to be assigned manually if Rigidbody or CharacterController is not on GameObject
    [SerializeField]
    private CharacterController _characterController; // = GetComponent<Rigidbody>() | needs to be assigned manually if Rigidbody or CharacterController is not on GameObject

    [Header("Manually Assigned Chaches")]
    [SerializeField]
    private Animator _animator; // needs to be assigned manually
    #endregion

    #region Propertys
    private bool _grounded = false;
    private bool _groundedJump = false;
    public Vector3 Position { get => transform.position; private set => transform.position = value; }
    public new Transform transform
    {
        get
        {
            if (_characterController) return _characterController.transform;
            else return base.transform;
        }
    }
    #endregion

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

    private void Update()
    {
        // Get Input
        if (!_isDead)
            GetInput();
        else if (_newDeltaMovePos != Vector3.zero) _newDeltaMovePos = Vector3.zero;

        UpdateGravity();
        UpdateMovement();
        UpdateRotation();
        UpdateAnimator();
        
        // 
        FreezeZPosition();
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void FreezeZPosition()
    {
        Vector3 varr = transform.position;
        varr.z = 0;
        transform.position = varr;
    }

    private void GroundCheck()
    {
        _grounded = Physics.Raycast(Position, Vector3.down, _groundCheckDistance);
        _groundedJump = Physics.Raycast(Position, Vector3.down, _JumpGroundCheckDistance);
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
            _currentGravity.y = Mathf.Max(-100, _currentGravity.y);
        }
        else if (_currentGravity != Vector3.zero)
        {
            _currentGravity = Vector3.zero;
        }
    }

    private void UpdateMovement()
    {
        _characterController.Move(Vector3.right * _newDeltaMovePos.x * _speed * Time.deltaTime + _currentGravity * Time.deltaTime * _jumpingFloat01);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.AngleAxis(_newDeltaMovePos.x * -90 + 180 , Vector3.up);
    }

    private void UpdateAnimator()
    {
        if (!_animator) return;
        if (_jumpCorutineTimer != 0f && _groundedJump)
        {
            _animator.SetBool("Grounded", true);
        }
        _animator.SetFloat("Speed_f", Mathf.Abs(_newDeltaMovePos.x) * 0.5f);
    }

    private void InputJump()
    {
        if (!_groundedJump || _jumpCorutineTimer != 0) return;
        Jump();
    }

    private void Jump()
    {
        if (_jumpingCoroutine != null) StopCoroutine(_jumpingCoroutine);
        _jumpingCoroutine = StartCoroutine(JumpingCorutine());
    }

    private IEnumerator JumpingCorutine()
    {
        if (_animator) _animator.SetBool("Jump_b", true);

        if (_jumpDelay != 0)
        {
            _jumpingFloat01 = 1f;
            yield return new WaitForSeconds(_jumpDelay);
        }

        if (_animator)
        {
            _animator.SetBool("Jump_b", false);
            _animator.SetBool("Grounded", false);
        }

        _jumpCorutineTimer = 0;
        while (_jumpCorutineTimer < 1f)
        {
            _jumpCorutineTimer += Time.deltaTime * _jumpSpeed;
            _jumpingFloat01 = _jumpCorutineTimer;
            _characterController.Move(Vector3.up * Time.deltaTime * _jumpHeight * _jumpCurve.Evaluate(_jumpCorutineTimer));
            _currentGravity = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
        _jumpCorutineTimer = 0;
        _jumpingFloat01 = 1f;
        yield return 0;
    }

    [ContextMenu("Die")]
    private void Die()
    {
        _isDead = true;
        _animator.SetBool("Death_b", true);
        //this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(Position, Position + Vector3.down * _groundCheckDistance);
    }

}

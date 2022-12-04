using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _rb2d;
    Vector2 _gravForce;
    Vector2 _rayDir = Vector2.down;

    bool _facingRight = true;
    bool _shouldMaintainHeight = true;

    public bool grounded { get; private set; }
    [SerializeField] GameObject _presentThrown;
    [SerializeField] Transform _throwPos;

    float _moveInput;

    bool _desiredJump;
    bool _isJumping;
    

    int _xDirect;

    [SerializeField] GameObject _sack;

    [Header("Config Values:")]
    [SerializeField] LayerMask _whatIsGround;

    [Header("Movement:")]
    [SerializeField] float _defaultMaxSpeed = 4f;
    [SerializeField] float _dashMaxSpeed = 8f;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _acceleration;

    [Header("Movement Modifiers:")]
    [SerializeField] float[] _movementSpeedTiers;
    [SerializeField] float[] _jumpGravityTiers;
    [SerializeField] int[] _maxPresentTiers;
    int _currentSpeedTier;

    Vector2 _desiredVelocity;
    Vector2 _velocity;
    float _maxSpeedChange;
    float _prevMoveInput;

    [Header("Height Spring:")]
    [SerializeField] float _rideHeight = 1.5f;
    [SerializeField] float _rayToGroundHeight = 3f;
    public float rideSpringStrength = 50f;
    [SerializeField] float _rideSpringDamp = 5f;

    //Jump Values
    [Header("Jump Values:")]
    [SerializeField] float _jumpHeight = 4f;
    [SerializeField] float _defaultJumpGrav = 1.7f;
    [SerializeField] float _defaultFallGrav = 3f;
    [SerializeField] float _jumpBuffer = .15f;
    [SerializeField] float _coyoteTime = .25f;
    [SerializeField] int maxJumps;
    float _jumpBufferCount;
    float _coyoteCounter;
    float _jumpPhase;
    bool _prevGrounded = false;
    bool _jumping;

    PlayerInputActions _input;


    //Add test counter for the presents
    int _presentCounter;
    int xDirect = 1;


    private void Awake()
    {
        _input = new PlayerInputActions();

        _input.Player.Enable();

        _input.Player.Jump.started += PlayerJump;
        _input.Player.Jump.canceled += PlayerJump;
        _input.Player.Throw.started += Throw;
        

        _rb2d = GetComponent<Rigidbody2D>();
        _gravForce = Physics2D.gravity * _rb2d.mass;
        _maxSpeed = _defaultMaxSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _desiredVelocity = new Vector2(_moveInput, 0) * Mathf.Max((_maxSpeed * _movementSpeedTiers[_currentSpeedTier]), 0);

        _desiredJump |= _input.Player.Jump.WasPressedThisFrame();
        //Debug.Log(_input.Player.Jump.IsPressed());
    }

    private void FixedUpdate()
    {
        (bool rayHitGround, RaycastHit2D hit) = RaycastToGround();

        grounded = CheckGrounded(rayHitGround, hit);

        //Debug.Log(grounded);

        if (grounded)
        {
            if (!_prevGrounded)
            {

            }
        }

        if(rayHitGround && _shouldMaintainHeight)
        {
            MaintainHeight(hit);
        }

        _velocity = _rb2d.velocity;


        if(_moveInput != 0)
        {
            _prevMoveInput = _moveInput;
        }

        //Add checks for input

        _moveInput = _input.Player.Movement.ReadValue<float>();

        if(_moveInput != _prevMoveInput && _moveInput != 0)
        {
            //Flip the x scale based on the new direction
            if(_moveInput > _prevMoveInput && !_facingRight)
            {
                FlipXScale();
            } else if(_moveInput < _prevMoveInput && _facingRight)
            {
                FlipXScale();
            }
        }

        PlayerMove();
        Jump();
        //Insert the jump method

        _rb2d.velocity = _velocity;
    }

    void PlayerMove()
    {
        _maxSpeedChange = _acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
    }

    private void MaintainHeight(RaycastHit2D hit)
    {
        Vector2 vel = _rb2d.velocity;
        Vector2 otherVel = Vector2.zero;

        Rigidbody2D hitrb2d = hit.rigidbody;

        if(hitrb2d != null)
        {

        }

        float rayDirVelocity = Vector2.Dot(_rayDir, vel);
        float otherDirVelocity = Vector2.Dot(_rayDir, otherVel);

        float relativeVel = rayDirVelocity - otherDirVelocity;
        float currHeight = hit.distance - _rideHeight;
        float springForce = (currHeight * rideSpringStrength) - (relativeVel * _rideSpringDamp);
        Vector2 maintainHeightForce = -_gravForce + springForce * Vector2.down;


        _rb2d.AddForce(maintainHeightForce);

        if(hitrb2d != null)
        {

        }

        

    }

    private bool CheckGrounded(bool rayHitGround, RaycastHit2D hit)
    {
        bool grounded = false;

        if (rayHitGround)
        {
            grounded = hit.distance < _rideHeight * 1.3f;
        }
        else
        {
            grounded = false;
        }

        return grounded;
    }

    private (bool rayHitGround, RaycastHit2D hit) RaycastToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundHeight, _whatIsGround);

        bool rayHitGround = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundHeight, _whatIsGround);

        return (rayHitGround, hit);
    }

    void Jump()
    {
        if (grounded)
        {
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCount = _jumpBuffer;
        } else if(!_desiredJump && _jumpBufferCount > 0)
        {
            _jumpBufferCount -= Time.deltaTime;
        }

        if(_jumpBufferCount > 0)
        {
            JumpAction();
        }
        if(_jumping && _velocity.y > 0)
        {
            _rb2d.gravityScale = (_defaultJumpGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = false;
        }

        if(!_jumping || _velocity.y < 0)
        {
            _rb2d.gravityScale = (_defaultFallGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = true;
        }

        if (grounded)
        {
            _rb2d.gravityScale = 1f;
        }

        //Add jump button to action map
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0 || (_isJumping && _jumpPhase < maxJumps))
        {

            _coyoteCounter = 0f;
            float jumpSpeed = Mathf.Sqrt(-2 * Physics2D.gravity.y * _jumpHeight);

           

            _isJumping = true;

            if (_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }

            _velocity.y += jumpSpeed;
        }
        
    }

    void PlayerJump(InputAction.CallbackContext context)
    {
        _jumping = !_jumping;
    }

    void IncreasePresentNumb()
    {
        _presentCounter++;
        CheckPresentCount(_presentCounter);
    }

    public void CheckPresentCount(int presentNo)
    {
        
        if (presentNo >= _maxPresentTiers[_currentSpeedTier])
        {
            _currentSpeedTier++;
            _sack.transform.localScale += new Vector3(0, 1, 0);
        } else if(_currentSpeedTier > 0)
        {
            if (presentNo < _maxPresentTiers[_currentSpeedTier - 1])
            {
                _currentSpeedTier--;
                _sack.transform.localScale += new Vector3(0, -1, 0);
            }
        }
    }

    void Throw(InputAction.CallbackContext context)
    {
        //Add the ability to throw DO IT NOW DO IT NOW
        if (GameManager.instance.presentCount > 0)
        {
            GameObject present = Instantiate(_presentThrown, _throwPos.position, transform.rotation);
            present.GetComponent<Rigidbody2D>().velocity = transform.right * 6f * xDirect;

            GameManager.instance.ChangePresentCount(-1);

        }
    }

    void FlipXScale()
    {
        _facingRight = !_facingRight;
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
        xDirect = -xDirect;
    }

    //Outline input methods
}

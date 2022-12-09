using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

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
    bool _isDashing;
    float _lastImageXPos;
    [SerializeField]
    float _distBetweenImages = .25f;
    [SerializeField]
    float _wallCheckDistance = 1.5f;

    int _xDirect;

    [SerializeField] GameObject _sack;
    [SerializeField] GameObject _feet;

    [Header("Config Values:")]
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] float _enemyStompLength;

    [Header("Movement:")]
    [SerializeField] float _defaultMaxSpeed = 4f;
    [SerializeField] float _dashMaxSpeed = 8f;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _acceleration;
    [SerializeField] float rollForce;
    [SerializeField] bool _rollCooldown;

    [Header("Movement Modifiers:")]
    [SerializeField] float[] _movementSpeedTiers;
    [SerializeField] float[] _jumpGravityTiers;
    [SerializeField] float[] _rollDistanceTiers;
    [SerializeField] int[] _maxPresentTiers;
    [SerializeField] float rollTime;
    [SerializeField] float _rollBuffer = .15f;
    int _currentSpeedTier;
    float _rollBufferCount;
    bool _desiredRoll = false;

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
    int remainingJumps;
    float _jumpBufferCount;
    float _coyoteCounter;
    float _jumpPhase;
    bool _prevGrounded = false;
    [HideInInspector]
    public bool _beingMoved;

    [Header("Slam Values:")]
    [SerializeField] float[] slamRadiusTiers;

    //Jumping used for input system checks
    bool _jumping;
    //Ability booleans (Whether they're being used or not)
    bool _rolling;
    bool _slamming;
    bool _cancelledRoll;
    bool _canRoll;

    [Header("Knockback Values:")]
    [SerializeField] float _knockTime;
    [HideInInspector] public bool _isKnocked = false;
    [SerializeField] Vector2 _knockForce;

    float dashModifier = 1f;
    public PlayerInputActions _input;
    

    SpriteRenderer _renderer;

    //Add test counter for the presents
    int _presentCounter;
    int xDirect = 1;
    int enemyStompCount = 1;

    private void Awake()
    {
        _input = new PlayerInputActions();

        _input.Player.Enable();

        
        _input.Player.Throw.started += Throw;


        _input.Player.Jump.performed += PlayerJump;
        _input.Player.Dash.performed += SetPlayerDash;
        _input.Player.Dash.canceled += SetPlayerDash;
        //_input.Player.Roll.performed += Roll;

        //_input.Player.Slam.started += Slam;

        _input.Player.ActionButton.performed += RollOrSlam;

        _canRoll = true;
        _rb2d = GetComponent<Rigidbody2D>();
        _gravForce = Physics2D.gravity * _rb2d.mass;
        _maxSpeed = _defaultMaxSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        remainingJumps = maxJumps;
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.gamePaused || _isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            return;
        }

        if (!_cancelledRoll)
        {
            _desiredVelocity = new Vector2(_moveInput, 0) * Mathf.Max(((_maxSpeed * _movementSpeedTiers[_currentSpeedTier]) * dashModifier), 0);
        }
        else
        {
            _desiredVelocity = new Vector2(Mathf.Abs(_rb2d.velocity.x) * xDirect, _rb2d.velocity.y);
        }

        bool cancelRoll = Physics2D.Raycast(transform.position, transform.right, .6f * xDirect, _whatIsGround);
        Debug.DrawRay(transform.position, transform.right * .6f * xDirect, Color.red);

        if(_cancelledRoll && cancelRoll)
        {
            EndRollCancel();
        }
        

        //Debug.Log(_desiredJump);
    }

    private void FixedUpdate()
    {
        if (GamePause.gamePaused || _beingMoved)
        {
            _rb2d.velocity = Vector2.zero;
            return;
        }

        if (_isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _slamming)
        {
            return;
        }

        (bool rayHitGround, RaycastHit2D hit) = RaycastToGround();

        grounded = CheckGrounded(rayHitGround, hit);

        //Debug.Log(grounded);

        if (grounded)
        {
            if (!_prevGrounded)
            {
                EndRollCancel();
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
        if(_moveInput > 0.01f)
        {
            _moveInput = 1;
        }
        else if(_moveInput < -.01f)
        {
            _moveInput = -1;
        }

        if(_moveInput != _prevMoveInput && _moveInput != 0)
        {
            //Flip the x scale based on the new direction
            if(_moveInput > 0 && !_facingRight)
            {
                FlipXScale();
            } else if(_moveInput < 0 && _facingRight)
            {
                FlipXScale();
            }
        }

        PlayerMove();
        Jump();
        RollAction();

        if (_isDashing)
        {
            if (MathF.Abs(transform.position.x - _lastImageXPos) > _distBetweenImages)
            {
                AfterImageObjectPool.instance.GetFromPool();
                _lastImageXPos = transform.position.x;
            }
        }

        _prevGrounded = grounded;

        _rb2d.velocity = _velocity;
    }

    private void EndRollCancel()
    {
        _cancelledRoll = false;
    }

    #region Movement, Height Maintenance

    void PlayerMove()
    {
        if (!_cancelledRoll)
        {
            _maxSpeedChange = _acceleration * GamePause.fixedDeltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        }
        else
        {
            _velocity.x = _desiredVelocity.x;
        }
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
    #endregion

    #region Jump
    void Jump()
    {
        if (grounded)
        {
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
            
            _jumpPhase = 0;
            enemyStompCount = 1;
        }
        else
        {
            _coyoteCounter -= GamePause.fixedDeltaTime;
        }

        if (_desiredJump)
        {
            
            _desiredJump = false;
            _jumpBufferCount = _jumpBuffer;
        } else if(!_desiredJump && _jumpBufferCount > 0)
        {
            _jumpBufferCount -= GamePause.fixedDeltaTime;
        }

        if(_jumpBufferCount > 0)
        {
            JumpAction();
        }
        if(_input.Player.Jump.ReadValue<float>() != 0 && _velocity.y > 0)
        {
            _rb2d.gravityScale = (_defaultJumpGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = false;
        }

        if(_input.Player.Jump.ReadValue<float>() == 0 || _velocity.y < 0)
        {
            _rb2d.gravityScale = (_defaultFallGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = true;
            CheckForEnemyStomp();
        }

        if (grounded)
        {
            _rb2d.gravityScale = 1f;
            remainingJumps = maxJumps;
        }

        //Add jump button to action map
    }

    void RollAction()
    {
        if (_desiredRoll)
        {
            _desiredRoll = false;
            _rollBufferCount = _rollBuffer;
        } else if(!_desiredRoll && _rollBufferCount > 0)
        {
            _rollBufferCount -= GamePause.fixedDeltaTime;
        }

        if(_rollBufferCount > 0)
        {
            CheckForRoll();
        }
    }

    void CheckForRoll()
    {
        if (grounded)
        {
            if (!_rolling && _canRoll)
            {
                if (_isDashing)
                {
                    StartCoroutine(PlayerRollCo(.28f));

                }
                else
                {
                    StartCoroutine(PlayerRollCo(.22f));
                }
            }
        }
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0 || (_isJumping && remainingJumps > 0))
        {
            
            _coyoteCounter = 0f;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            _isJumping = true;

            if (_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }

            _velocity.y += jumpSpeed;
        }
    }

    #endregion

    #region Player Input Checks

    void PlayerJump(InputAction.CallbackContext context)
    {


        if (GamePause.gamePaused || _isKnocked || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming || _isKnocked)
        {
            return;
        }

        if (!context.performed)
        {
            return;
        }


        if (_rolling)
        {
            _rolling = false;
            _cancelledRoll = true;

            Debug.Log(_rb2d.velocity.x);
                
            //_rb2d.velocity = new Vector2(5 * xDirect, _rb2d.velocity.y);
            

            GetComponent<PlayerRoll>().PlayerRollAction(false);
            StartCoroutine(RollCooldownCo());
            Invoke("EndRollCancel", 1.2f);
            //Note: Possibly add coroutine to handle the roll cancel momentum ending separately
        }

        _desiredJump = true;

        if (_isJumping)
        {
            remainingJumps--;
            if (_cancelledRoll)
            {
                StartCoroutine(CancelledRollEndCo());
            }
        }
    }

    

    void SetPlayerDash(InputAction.CallbackContext context)
    {
        if (GamePause.gamePaused || _isKnocked || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            _isDashing = false;

            return;
        }

        if (context.performed)
        {
            //Set the player to dash
            dashModifier = 1.5f;
            _isDashing = true;
            AfterImageObjectPool.instance.GetFromPool();
            _lastImageXPos = transform.position.x;
        }
        else if (context.canceled)
        {
            //Cancel the dash (Set modifier back to normal)
            dashModifier = 1f;
            _isDashing = false;
        }
    }

    void Throw(InputAction.CallbackContext context)
    {
        if (GamePause.gamePaused || _isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            return;
        }

        //Add the ability to throw DO IT NOW DO IT NOW
        if (GameManager.instance.presentCount > 0)
        {
            
            GameObject present = PresentObjectPool.instance.GetFromPool();
            present.GetComponent<PresentObject>().Invoke("EnableCollider", .5f);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * xDirect, _wallCheckDistance, _whatIsGround);

            if (hit)
            {
                present.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
                present.GetComponent<PresentObject>()._droppedOrThrown = true;
                present.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.right.x * xDirect, .7f) * 6;
            }
            else
            {
                present.transform.position = _throwPos.position;
                present.GetComponent<PresentObject>()._droppedOrThrown = true;
                present.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.right.x * xDirect, .5f) * 6f;
            }

            
            GameManager.instance.ChangePresentCount(-1);

        }
    }

    public void Knockback(Vector2 aiPosition)
    {
        Vector2 knockDir = (Vector2)transform.position - aiPosition;
        knockDir = knockDir.normalized;
        knockDir.y = .3f;
        StartCoroutine(KnockbackCo(knockDir));
    }

    IEnumerator KnockbackCo(Vector2 knockDir)
    {
        float knockTimer = _knockTime;

        while(knockTimer > 0)
        {
            if (!_isKnocked)
            {
                _isKnocked = true;
                _rb2d.velocity = new Vector2(knockDir.x * _knockForce.x, knockDir.y * _knockForce.y);
            }

            knockTimer -= GamePause.deltaTime;
            yield return null;
        }

        _isKnocked = false;
    }

    public void CheckForEnemyStomp()
    {
        
        bool enemyStomp = Physics2D.OverlapBox(_feet.transform.position, new Vector2(.95f, .13f), 0, _enemyLayer);
        if (enemyStomp)
        {
            AIThinker enemy = Physics2D.OverlapBox(_feet.transform.position, new Vector2(.5f, .5f), 0, _enemyLayer).GetComponent<AIThinker>();
            StartCoroutine(CinemachineCamShake.CamShakeCo(.1f, FindObjectOfType<CinemachineVirtualCamera>()));
            enemy.isStunned = true;
            _velocity.y += 2f * enemyStompCount * 1.5f;

            if (enemyStompCount < 3)
            {
                enemyStompCount++;
            }
        }

    }

    void RollOrSlam(InputAction.CallbackContext context)
    {
        if (_rolling || _isKnocked || GamePause.gamePaused || GameManager.instance.isCountingDown || !_canRoll || UIFade.instance.fading || _beingMoved || _slamming)
        {
            return;
        }

        if (_currentSpeedTier < 2)
        {
            //Roll
            Roll(context);
        }
        else
        {
            //Slam
            Slam(context);
        }
    }

    void Roll(InputAction.CallbackContext context)
    {


        
        if (context.performed)
        {
            _desiredRoll = true;


            if (grounded)
            {
                /*
                if (_isDashing)
                {
                    StartCoroutine(PlayerRollCo(.38f));
                    
                }
                else
                {
                    StartCoroutine(PlayerRollCo(.22f));
                }*/
            }
            
        }
    }

    IEnumerator PlayerRollCo(float rollSpeed)
    {
        GetComponent<PlayerRoll>().PlayerRollAction(true);
        _renderer.color = Color.yellow;

        float rollTimer = rollTime;

        _rolling = true;

        while(rollTimer > 0 && _rolling)
        {
            
            rollTimer -= GamePause.deltaTime;

            _rb2d.AddForce(new Vector2(rollSpeed * rollForce * xDirect * _rollDistanceTiers[_currentSpeedTier] * GamePause.deltaTime, 0), ForceMode2D.Impulse);

            yield return null;
        }

        _renderer.color = Color.white;

        _rolling = false;

        if (_rollCooldown)
        {
            _canRoll = false;

            StartCoroutine(RollCooldownCo());
        }

        GetComponent<PlayerRoll>().PlayerRollAction(false);

    }

    void Slam(InputAction.CallbackContext context)
    {
        if (GamePause.gamePaused || GameManager.instance.isCountingDown || UIFade.instance.fading || _rolling || _beingMoved || _isKnocked)
        {
            return;
        }

        //Check if the player's bag has reached a certain threshold
        if (grounded)
        {
            if (_currentSpeedTier >= 2)
            {
                _renderer.color = Color.blue;

                Debug.Log("SLAM!");



                Collider2D[] slammedEnemies = Physics2D.OverlapCircleAll(transform.position, slamRadiusTiers[_currentSpeedTier], _enemyLayer);

                if (slammedEnemies.Length != 0)
                {
                    for (int i = 0; i < slammedEnemies.Length; i++)
                    {
                        AIThinker enemyToSlam = slammedEnemies[i].GetComponent<AIThinker>();

                        if (!enemyToSlam.isBeingLaunched)
                        {
                            enemyToSlam.isStunned = true;
                            StartCoroutine(LaunchEnemyCo(enemyToSlam));
                        }

                    }
                }
                else
                {
                    Debug.Log("AAAAAA");
                }

                Collider2D[] slammedDestructibles = Physics2D.OverlapCircleAll(transform.position, slamRadiusTiers[_currentSpeedTier], _whatIsGround);

                if(slammedDestructibles != null)
                {
                    for(int i = 0; i < slammedDestructibles.Length; i++)
                    {
                        IDamageInterface objectToDamage = slammedDestructibles[i].GetComponent<IDamageInterface>();

                        if(objectToDamage != null)
                        {
                            objectToDamage.DamageObject(1);
                        }
                    }
                }


                StartCoroutine(CinemachineCamShake.CamShakeCo(.14f, FindObjectOfType<CinemachineVirtualCamera>()));
                StartCoroutine(SetSlamFinishCo());
            }
        }


    }

    IEnumerator LaunchEnemyCo(AIThinker enemyToLaunch)
    {
        enemyToLaunch.isBeingLaunched = true;
        yield return new WaitForSeconds(.1f);
        Vector2 slamKnockDir = enemyToLaunch.transform.position - transform.position;
        slamKnockDir.y = .5f;
        slamKnockDir = slamKnockDir.normalized;

        if (enemyToLaunch.grounded)
        {
            enemyToLaunch.rb2d.velocity = new Vector2(slamKnockDir.x * 10, slamKnockDir.y * 20);
        }

        yield return new WaitForSeconds(.3f);
        enemyToLaunch.isBeingLaunched = false;
        enemyToLaunch.rb2d.velocity = Vector2.zero;
        
    }

    IEnumerator SetSlamFinishCo()
    {
        yield return new WaitForSeconds(.2f);
        _slamming = false;
        _renderer.color = Color.white;
    }

    IEnumerator RollCooldownCo()
    {
        yield return new WaitForSeconds(.27f);
        _canRoll = true;
        //_cancelledRoll = false;
    }

    IEnumerator CancelledRollEndCo()
    {
        float cancelRollTimer = .6f;

        while(cancelRollTimer > 0)
        {
            cancelRollTimer -= GamePause.deltaTime;

            yield return null;
        }

        _cancelledRoll = false;
    }

    #endregion

    #region Misc
    void FlipXScale()
    {
        _facingRight = !_facingRight;
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
        xDirect = -xDirect;
    }

    public void CheckPresentCount(int presentNo)
    {

        if (presentNo >= _maxPresentTiers[_currentSpeedTier])
        {
            _currentSpeedTier++;
            _sack.transform.localScale += new Vector3(0, 1, 0);
        }
        else if (_currentSpeedTier > 0)
        {
            if (presentNo < _maxPresentTiers[_currentSpeedTier - 1])
            {
                _currentSpeedTier--;
                _sack.transform.localScale += new Vector3(0, -1, 0);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, slamRadiusTiers[_currentSpeedTier]);
    }

    #endregion
    //Outline input methods
}

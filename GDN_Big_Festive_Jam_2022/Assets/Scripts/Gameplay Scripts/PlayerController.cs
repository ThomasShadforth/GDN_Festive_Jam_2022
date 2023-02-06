using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    //Reference to the player's rigidbody, publicly accessed via the player animator script
    public Rigidbody2D _rb2d;
    //The force of gravity which is applied to the player
    Vector2 _gravForce;
    //The direction for the raycast used to detec the ground
    Vector2 _rayDir = Vector2.down;


    //Whether or not the player is facing right or not
    bool _facingRight = true;
    //Whether the height of the player's floating capsule should be maintained
    bool _shouldMaintainHeight = true;

    //Whether or not the player is grounded
    public bool grounded { get; private set; }
    //The position from which the player throws picked up presents
    [SerializeField] Transform _throwPos;

    //The value of the moveInput received from the player's keyboard/controller
    float _moveInput;

    //Whether the player wants to jump, is jumping, is dashing
    bool _desiredJump;
    bool _isJumping;
    bool _isDashing;
    //Values relating to the dashing after image effect (last position, distance between images)
    float _lastImageXPos;
    [SerializeField]
    float _distBetweenImages = .25f;
    //Distance to check between the player and the wall (used for jump cancelling the roll)
    [SerializeField]
    float _wallCheckDistance = 1.5f;


    int _xDirect;

    //The bag the player carries (The original test bag). Resized based on the threshold crossed
    [SerializeField] GameObject _sack;
    //Reference to the an object serving as the feet. Checks whether or not the player is jumping on an enemy
    [SerializeField] GameObject _feet;

    [Header("Config Values:")]
    //The ground layer (For ground detection), the enemy layer (For slamming/stomping), and how long the raycast for stomping is
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] float _enemyStompLength;

    [Header("Movement:")]
    //Default move speed, the speed while dashing
    [SerializeField] float _defaultMaxSpeed = 4f;
    [SerializeField] float _dashMaxSpeed = 8f;
    [SerializeField] float _maxSpeed;
    //The current acceleration
    [SerializeField] float _acceleration;
    //The force applied to the player while rolling, and how long the roll is on cooldown for
    [SerializeField] float rollForce;
    [SerializeField] bool _rollCooldown;

    [Header("Movement Modifiers:")]
    //Stores a number of different modifiers that are applied to move speed, jumping, and rolling. These change based on how many presents the player collects, and whether or not any thresholds are crossed while doing so
    [SerializeField] float[] _movementSpeedTiers;
    [SerializeField] float[] _jumpGravityTiers;
    [SerializeField] float[] _rollDistanceTiers;
    [SerializeField] Vector2[] _sackSizeTiers;
    //Stores gameobjects for the sack the player carries. Enables/Disables based on the threshold crossed (Small, medium, large)
    [SerializeField] GameObject[] _sackObjects;
    //Determines how many presents the player must collect before the bag gets heavier
    [SerializeField] int[] _maxPresentTiers;
    //How long the player rolls for
    [SerializeField] float rollTime;
    //Buffer time for the roll Input. Used to briefly check multiple times. If pressed just before landing, then the player will roll (Helps to feel more responsive)
    [SerializeField] float _rollBuffer = .15f;
    //Keeps track of the current weight tier
    int _currentSpeedTier;
    //Stores the buffer to use as a timer
    float _rollBufferCount;
    //Whether or not the player wants to roll
    bool _desiredRoll = false;

    //The overall desired velocity the player object is trying to reach
    Vector2 _desiredVelocity;
    //The current velocity
    Vector2 _velocity;
    //Max speed change is determined by the acceleration over time
    float _maxSpeedChange;
    //Records the previous direction of input. Used for switching sprite directions
    float _prevMoveInput;

    //Values related to the height spring, which both checks for the ground and keeps the player capsule floating (Allowing it to work with slopes without having to write additional code for handling slope movement)
    [Header("Height Spring:")]
    //How high the capsule sits
    [SerializeField] float _rideHeight = 1.5f;
    //The distance of the check between the player and the ground. Must Always be greater than ride height, otherwise it will cause bugs
    [SerializeField] float _rayToGroundHeight = 3f;
    //The strength of the ride spring when shifting the player object to the height of the spring
    public float rideSpringStrength = 50f;
    [SerializeField] float _rideSpringDamp = 5f;

    //Jump Values
    [Header("Jump Values:")]
    //The jump height
    [SerializeField] float _jumpHeight = 4f;
    //Different gravity values for jumping and falling. Allows for jumps to be tweaked in order to be exaggerated, or to feel more realistic depending on the requirements
    [SerializeField] float _defaultJumpGrav = 1.7f;
    [SerializeField] float _defaultFallGrav = 3f;
    //Buffer value for jump input. Much like the roll, will check multiple times while the corresponding timer is greater than 0. Allows for a jump to be performed if pressed just before hitting the ground
    [SerializeField] float _jumpBuffer = .15f;
    //Coyote time determines the amount of the time the player is still able to jump normally after walking off of an edge. Gives a brief window
    [SerializeField] float _coyoteTime = .25f;
    //The max number of jumps the player may perform
    [SerializeField] int maxJumps;
    int remainingJumps;
    //Respective timers for jump buffer and coyote time
    float _jumpBufferCount;
    float _coyoteCounter;
    float _jumpPhase;
    //Whether or not the player was previously grounded (Can be used for a landing animation/particle effect)
    bool _prevGrounded = false;
    //Used to prevent movement while the player is being moved after falling in a death pit
    [HideInInspector]
    public bool _beingMoved;

    //How large the player's slam radius is at the heavier bag weights
    [Header("Slam Values:")]
    [SerializeField] float[] slamRadiusTiers;

    //Jumping used for input system checks
    [HideInInspector]
    public bool _jumping;
    //Ability booleans (Whether they're being used or not)
    [HideInInspector]
    public bool _rolling;
    [HideInInspector]
    bool _slamming;
    //Whether the roll has been jump cancelled or not
    bool _cancelledRoll;
    //Whether or not the player is able to roll
    bool _canRoll;

    [Header("Knockback Values:")]
    [SerializeField] float _knockTime;
    [HideInInspector] public bool _isKnocked = false;
    [SerializeField] Vector2 _knockForce;

    float dashModifier = 1f;
    //Creates an input action map reference in order to enable this script for use with the input system
    public PlayerInputActions _input;

    PlayerAnimator _playerAnimator;

    SpriteRenderer _renderer;

    //Add test counter for the presents
    int _presentCounter;
    int xDirect = 1;
    int enemyStompCount = 1;

    private void Awake()
    {
        //Enables input action map
        _input = new PlayerInputActions();

        _input.Player.Enable();

        //Assign methods to input actions being performed for throwing presents, dashing, slamming, etc.
        _input.Player.Throw.started += Throw;


        _input.Player.Jump.performed += PlayerJump;
        _input.Player.Dash.performed += SetPlayerDash;
        _input.Player.Dash.canceled += SetPlayerDash;
        //_input.Player.Roll.performed += Roll;

        //_input.Player.Slam.started += Slam;

        _input.Player.ActionButton.performed += RollOrSlam;

        //Player can roll by default
        _canRoll = true;
        _rb2d = GetComponent<Rigidbody2D>();

        //Sets the gravitational force applied to the player when landing
        _gravForce = Physics2D.gravity * _rb2d.mass;
        //Set the maxSpeed to the default
        _maxSpeed = _defaultMaxSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set remaining jumps, and get the spriteRenderer and animator component/script
        remainingJumps = maxJumps;
        _renderer = GetComponent<SpriteRenderer>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        //Sets initial bag size (Used for test bag)
        SetInitialSize();

    }

    void SetInitialSize()
    {
        _sack.transform.localScale = _sackSizeTiers[_currentSpeedTier];

        for(int i = 0; i < _sackObjects.Length; i++)
        {
            if(i != _currentSpeedTier)
            {
                _sackObjects[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Do not execute when paused, rolling, etc.
        if (GamePause.gamePaused || _isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            
            return;
        }

        //Get desired velocity based on the player's input
        
        if (!_cancelledRoll)
        {
            _desiredVelocity = new Vector2(_moveInput, 0) * Mathf.Max(((_maxSpeed * _movementSpeedTiers[_currentSpeedTier]) * dashModifier), 0);
        }
        //Maintain velocity from a roll if jump cancelling it
        else
        {
            _desiredVelocity = new Vector2(Mathf.Abs(_rb2d.velocity.x) * xDirect, _rb2d.velocity.y);
        }

        //Checks for walls while in the jump cancelled roll state. If both conditions are met, allows normal movement again (Prevents an issue where the player could get stuck on a wall)
        bool cancelRoll = Physics2D.Raycast(transform.position, transform.right, .6f * xDirect, _whatIsGround);
        

        if(_cancelledRoll && cancelRoll)
        {
            EndRollCancel();
        }
        
        

        //Debug.Log(_desiredJump);
    }

    private void FixedUpdate()
    {
        //Checks if the ray is hitting the ground, and gets the object being detected
        (bool rayHitGround, RaycastHit2D hit) = RaycastToGround();

        //Checks if the player is properly grounded
        grounded = CheckGrounded(rayHitGround, hit);

        //Sets velocity to 0 if the game is paused, or if the player is being moved from a death pit
        if (GamePause.gamePaused || _beingMoved)
        {
            _rb2d.velocity = Vector2.zero;
            _rb2d.gravityScale = 0f;
            return;
        }

        //Otherwise, do not run while being knocked back, rolling, etc.
        if (_isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _slamming)
        {
            return;
        }

        

        //Debug.Log(grounded);

        //If the player lands on the ground and was not grounded on the previous frame, ends the roll cancel if they were in that state prior
        if (grounded)
        {
            if (!_prevGrounded)
            {
                EndRollCancel();
            }
        }

        //Maintains the height of the spring if the ray is hitting the ground
        if(rayHitGround && _shouldMaintainHeight)
        {
            MaintainHeight(hit);
        }
        //Sets velocity to the rigidbody's current velocity

        _velocity = _rb2d.velocity;

        //If move input is not 0, then set the previous input to the current input
        if(_moveInput != 0)
        {
            _prevMoveInput = _moveInput;
        }

        //Add checks for input

        //Reads the float value from the player's horizontal input via the action map
        _moveInput = _input.Player.Movement.ReadValue<float>();
        //If using a controller and pressing downward diagonal for either left or right, sets it to a default value (Prevents slow movement)
        if(_moveInput > 0.01f)
        {
            _moveInput = 1;
        }
        else if(_moveInput < -.01f)
        {
            _moveInput = -1;
        }

        //If the player is currently moving and the input is not the same as the last direction, flip the xScale based on the new direction
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

        //Move the player
        PlayerMove();
        //The player's jump
        Jump();
        //The player's roll action
        RollAction();

        //If the player is holding the dash/sprint button, move afterImage object to player's position from the object pool
        if (_isDashing)
        {
            if (MathF.Abs(transform.position.x - _lastImageXPos) > _distBetweenImages)
            {
                AfterImageObjectPool.instance.GetFromPool();
                _lastImageXPos = transform.position.x;
            }
        }

        //Sets previous grounded to whether or not the player was grounded this frame
        _prevGrounded = grounded;

        //Sets the rigidbody's velocity to the new velocity value
        _rb2d.velocity = _velocity;
    }

    //Ends the roll cancel by setting it to false
    private void EndRollCancel()
    {
        _cancelledRoll = false;
    }

    #region Movement, Height Maintenance

    void PlayerMove()
    {
        if (!_cancelledRoll)
        {
            //Max speed change is calculated over time
            _maxSpeedChange = _acceleration * GamePause.fixedDeltaTime;
            //Velocity moves from the current velocity to the desired velocity, using the max speed change to affect how quickly this happens
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        }
        //If the player has jump cancelled a roll, simply set velocity to the desired velocity
        else
        {
            _velocity.x = _desiredVelocity.x;
        }
    }

    //Method to maintain the height of the player's floating capsule
    private void MaintainHeight(RaycastHit2D hit)
    {
        //Gets the velocity of the player
        Vector2 vel = _rb2d.velocity;
        //Sets the velocity of a recipient object to 0
        Vector2 otherVel = Vector2.zero;
        //Gets the rigidbody of the recipient object if it has one
        Rigidbody2D hitrb2d = hit.rigidbody;

        if(hitrb2d != null)
        {

        }
        //Sets the directional velocity of the player's ray to the dot product of the direction (down) and the player's current velocity
        float rayDirVelocity = Vector2.Dot(_rayDir, vel);
        //Sets directional velocity for the other object to the dot product of down and the othervelocity
        float otherDirVelocity = Vector2.Dot(_rayDir, otherVel);

        //Gets the relative velocity by subtracting the other velocity from the player's directional velocity
        float relativeVel = rayDirVelocity - otherDirVelocity;
        //Gets the current height by subtracting the ride height from the distance between the player and the hit surface
        float currHeight = hit.distance - _rideHeight;
        //Subtracts the dampened velocity from the product of height * strength in order to get the overall force that needs to be applied
        float springForce = (currHeight * rideSpringStrength) - (relativeVel * _rideSpringDamp);
        //The final force is calculated by multiplying springforce by down, then adding it to the inverse of the gravitational force (Which will push the object upwards)
        Vector2 maintainHeightForce = -_gravForce + springForce * Vector2.down;

        //Adds the force to the player
        _rb2d.AddForce(maintainHeightForce);

        if(hitrb2d != null)
        {

        }

        

    }

    private bool CheckGrounded(bool rayHitGround, RaycastHit2D hit)
    {
        //Initialises grounded
        bool grounded = false;


        //If a hit has been made, check if the distance to the object is less than the ride height * 1.6. returns true or false. Otherwise, set grounded to false
        if (rayHitGround)
        {
            grounded = hit.distance < _rideHeight * 1.6f;
        }
        else
        {
            grounded = false;
        }

        return grounded;
    }

    //Uses the rayToGround to check if a surface on the ground layer exists beneath the player
    private (bool rayHitGround, RaycastHit2D hit) RaycastToGround()
    {
        //Gets the hit from the surface (if it exists on the ground layer)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundHeight, _whatIsGround);
        Debug.DrawRay(transform.position, Vector2.down * _rayToGroundHeight, Color.red);
        //Simultaneously sets the boolean based on this result
        bool rayHitGround = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundHeight, _whatIsGround);

        return (rayHitGround, hit);
    }
    #endregion

    #region Jump
    void Jump()
    {
        //Sets the coyote counter to it's default while grounded, sets isJumping to false, jumpPhase to 0, etc.
        if (grounded)
        {
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
            
            _jumpPhase = 0;
            enemyStompCount = 1;
        }
        else
        {
            //Subtracts coyote time as soons as the player is no longer grounded
            _coyoteCounter -= GamePause.fixedDeltaTime;
        }

        //If the player wishes to jump, sets it to false, and sets the timer for the buffer to the buffer value
        if (_desiredJump)
        {
            
            _desiredJump = false;
            _jumpBufferCount = _jumpBuffer;
        } else if(!_desiredJump && _jumpBufferCount > 0)
        {
            //Counts down over time
            _jumpBufferCount -= GamePause.fixedDeltaTime;
        }

        //Will call the jump action method while the buffer timer is greater than 0
        if(_jumpBufferCount > 0)
        {
            JumpAction();
        }
        //If jump is currently being held and velocity is greater than 0, gravity is set to jump gravity, and maintain height is set to false
        if(_input.Player.Jump.ReadValue<float>() != 0 && _velocity.y > 0)
        {
            _rb2d.gravityScale = (_defaultJumpGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = false;
        }

        //If jump is released or the velocity falls below 0, then falling gravity is applied. Whilst this is happening, the player's feet will check for stomping interactions
        if(_input.Player.Jump.ReadValue<float>() == 0 || _velocity.y < 0)
        {
            _rb2d.gravityScale = (_defaultFallGrav * _jumpGravityTiers[_currentSpeedTier]);
            _shouldMaintainHeight = true;
            CheckForEnemyStomp();
        }
        //When grounded, set gravity to 1, and remaining jumps to the max jump
        if (grounded)
        {
            _rb2d.gravityScale = 1f;
            remainingJumps = maxJumps;
        }

        
    }

    //Functions similarly to jump action (Executes check for roll while buffer time is greater than 0)
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

    //If the player is grounded, and currently isn't rolling and can roll, then they will roll (The speed is depending on whether they are currently rolling or not)
    void CheckForRoll()
    {
        if (grounded)
        {
            if (!_rolling && _canRoll)
            {
                if (_isDashing)
                {
                    StartCoroutine(PlayerRollCo(.289f));

                }
                else
                {
                    StartCoroutine(PlayerRollCo(.22f));
                }
            }
        }
    }

    //if the coyote counter is still active, or the player is already in the air and has remaining jumps
    private void JumpAction()
    {
        if (_coyoteCounter > 0 || (_isJumping && remainingJumps > 0))
        {
            //Gets jump speed based on the square root of -2 * gravity * jumpheight
            _coyoteCounter = 0f;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            //sets is jumping to true
            _isJumping = true;

            //If the player is already jumping and their velocity is greater than 0, sets jump speed based on which value is higher
            if (_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }
            //Adds the speed to the y velocity
            _velocity.y += jumpSpeed;
        }
    }

    #endregion

    #region Player Input Checks
    //Input check used to detect whether the player wants to jump while rolling. If they do, rolling is set to false, and cancelledRoll is set to trye
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
            
            //Passes false into the playerRoll's roll method (Sets the bag and the default colliders to active)
            GetComponent<PlayerRoll>().PlayerRollAction(false);
            //Starts the cooldown for the roll action
            StartCoroutine(RollCooldownCo());
            //At the same time, invokes the method to end the roll cancel state after a brief period of time
            Invoke("EndRollCancel", 1.2f);
            //Note: Possibly add coroutine to handle the roll cancel momentum ending separately
        }

        //Set desired jump to true
        _desiredJump = true;

        //If already jumping, double jump, and if cancelled roll is true, end the roll cancel
        if (_isJumping)
        {
            GetComponent<Animator>().SetBool("isDoubleJumping", true);
            remainingJumps--;
            if (_cancelledRoll)
            {
                StartCoroutine(CancelledRollEndCo());
            }
        }
    }

    
    //Sets whether or not the player is dashing based on if they are holding the input
    void SetPlayerDash(InputAction.CallbackContext context)
    {
        //Do not execute while paused
        if (GamePause.gamePaused || _isKnocked || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            _isDashing = false;

            return;
        }

        //If the button is pressed, set the player to dash, and start getting afterimages from the object pool
        if (context.performed)
        {
            //Set the player to dash
            dashModifier = 1.5f;
            _isDashing = true;
            AfterImageObjectPool.instance.GetFromPool();
            GetComponent<Animator>().SetBool("isDashing", true);
            _lastImageXPos = transform.position.x;
        }
        else if (context.canceled)
        {
            //Cancel the dash (Set modifier back to normal)
            dashModifier = 1f;
            GetComponent<Animator>().SetBool("isDashing", false);
            _isDashing = false;
        }
    }

    //Throws a held present
    void Throw(InputAction.CallbackContext context)
    {
        if (GamePause.gamePaused || _isKnocked || _rolling || GameManager.instance.isCountingDown || UIFade.instance.fading || _beingMoved || _slamming)
        {
            return;
        }

        //If the gamemanager's count for presents is greater than 0
        if (GameManager.instance.presentCount > 0)
        {
            //Get the present from the object pool (set it to active)
            GameObject present = PresentObjectPool.instance.GetFromPool();
            //Re-enable colliders after a short period of time
            present.GetComponent<PresentObject>().Invoke("EnableCollider", .95f);

            //Check whether the player is hitting a wall or not
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * xDirect, _wallCheckDistance, _whatIsGround);

            //If they are stood next to a wall, the present will be thrown above them instead of infront (Prevents it from being thrown through walls)
            if (hit)
            {
                present.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
                present.GetComponent<PresentObject>()._droppedOrThrown = true;
                present.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.right.x * xDirect, .7f) * 6;
            }
            //Otherwise, throw in front as intended
            else
            {
                present.transform.position = _throwPos.position;
                present.GetComponent<PresentObject>()._droppedOrThrown = true;
                present.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.right.x * xDirect, .7f) * 6f;
            }

            
            GameManager.instance.ChangePresentCount(-1);

        }
    }

    //Applies knockback in a direction based on whether an enemy is stood
    public void Knockback(Vector2 aiPosition)
    {
        //Calculate the direction
        Vector2 knockDir = (Vector2)transform.position - aiPosition;
        knockDir = knockDir.normalized;
        knockDir.y = .3f;
        //Pass direction into coroutine
        StartCoroutine(KnockbackCo(knockDir));
    }

    IEnumerator KnockbackCo(Vector2 knockDir)
    {
        //Push the player back in the knockback direction
        //Set the timer
        float knockTimer = _knockTime;

        while(knockTimer > 0)
        {
            if (!_isKnocked)
            {
                //Set the velocity to be the direction * force while the timer is greater than 0
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
        //If an enemy has been detected
        
        bool enemyStomp = Physics2D.OverlapBox(_feet.transform.position, new Vector2(.95f, .13f), 0, _enemyLayer);
        if (enemyStomp)
        {
            //Check for their collider
            Collider2D enemy = Physics2D.OverlapBox(_feet.transform.position, new Vector2(.5f, .5f), 0, _enemyLayer);

            //if it's not null, check if they have the AI thinker component
            if (enemy != null)
            {
                if (enemy.GetComponent<AIThinker>())
                {
                    //Store a reference to the stomped enemy
                    AIThinker enemyAI = enemy.GetComponent<AIThinker>();
                    //Applies a camera shake effect to the main camera when stomping on an enemy
                    StartCoroutine(CinemachineCamShake.CamShakeCo(.1f, FindObjectOfType<CinemachineVirtualCamera>()));
                    //Stuns the AI (prevents them from moving)
                    enemyAI.isStunned = true;
                    //Gives the player a small bounce after stomping on the enemy
                    _velocity.y += 2f * enemyStompCount * 1.5f;

                    //increases stomp count if it hasn't reached a limit (Used as a modifier for velocity)
                    if (enemyStompCount < 3)
                    {
                        enemyStompCount++;
                    }
                }
            }
        }

    }

    //Determines whether or not a player will roll or slam depending on their weight tier when pressing the action button
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

        //Desired roll is set to true
        
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

    //Applies the force to make the player roll
    IEnumerator PlayerRollCo(float rollSpeed)
    {
        //Disable normal colliders and the bag, enable the roll collider which is smaller
        GetComponent<PlayerRoll>().PlayerRollAction(true);
        //_renderer.color = Color.yellow;
        //Play the roll animation
        GetComponent<Animator>().Play("Roll");

        //Sets the roll timer
        float rollTimer = rollTime;

        //Rolling is set to true
        _rolling = true;

        //Applies the force over time while the timer is greater than 0
        while(rollTimer > 0 && _rolling)
        {
            
            rollTimer -= GamePause.deltaTime;

            _rb2d.AddForce(new Vector2(rollSpeed * rollForce * xDirect * _rollDistanceTiers[_currentSpeedTier] * GamePause.deltaTime, 0), ForceMode2D.Impulse);

            yield return null;
        }

        _renderer.color = Color.white;

        GetComponent<Animator>().Play("Idle");

        //Stops the roll once the timer reaches 0, and re-enables default colliders
        _rolling = false;

        if (_rollCooldown)
        {
            _canRoll = false;

            StartCoroutine(RollCooldownCo());
        }

        GetComponent<PlayerRoll>().PlayerRollAction(false);

    }

    //Produces a slam attack when pressing the action button while the bag is at it's heaviest
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

                //Checks for enemies within a radius based on the bag's weight (Heavier = larger)
                Collider2D[] slammedEnemies = Physics2D.OverlapCircleAll(transform.position, slamRadiusTiers[_currentSpeedTier], _enemyLayer);

                //If enemies have been detected
                if (slammedEnemies.Length != 0)
                {
                    //If the AI isn't being launched, stun them and then launch them with the slam
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
                
                //At the same time, check for any destructible crates

                Collider2D[] slammedDestructibles = Physics2D.OverlapCircleAll(transform.position, slamRadiusTiers[_currentSpeedTier], _whatIsGround);

                if(slammedDestructibles != null)
                {
                    //Get the interface for the destructiblle object, then call it's damage method
                    for(int i = 0; i < slammedDestructibles.Length; i++)
                    {
                        IDamageInterface objectToDamage = slammedDestructibles[i].GetComponent<IDamageInterface>();

                        if(objectToDamage != null)
                        {
                            objectToDamage.DamageObject(1);
                        }
                    }
                }

                //Apply the camera shake effect to the main camera, then end the slam
                StartCoroutine(CinemachineCamShake.CamShakeCo(.14f, FindObjectOfType<CinemachineVirtualCamera>()));
                StartCoroutine(SetSlamFinishCo());
            }
        }


    }

    IEnumerator LaunchEnemyCo(AIThinker enemyToLaunch)
    {
        //Set the ai's launched variable to true
        enemyToLaunch.isBeingLaunched = true;
        //Get the direction to launch the enemy in
        yield return new WaitForSeconds(.1f);
        Vector2 slamKnockDir = enemyToLaunch.transform.position - transform.position;
        slamKnockDir.y = .5f;
        slamKnockDir = slamKnockDir.normalized;

        //If they are grounded, launch them properly
        if (enemyToLaunch.grounded)
        {
            enemyToLaunch.rb2d.velocity = new Vector2(slamKnockDir.x * 25, slamKnockDir.y * 35);
        }

        //Stop launching the enemy, and set their velocity to 0
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
    
    //Re-enables the roll after a brief period
    IEnumerator RollCooldownCo()
    {
        yield return new WaitForSeconds(.27f);
        _canRoll = true;
        //_cancelledRoll = false;
    }

    //Used to end the jump-cancel for the roll action
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

    //Used to check the current number of present (Called from the game manager after picking up a present
    public void CheckPresentCount(int presentNo)
    {
        //If it exceeds a certain amount when picking up, then increase the weight, and set the corresponding bag to active (reduces speed)
        if (presentNo >= _maxPresentTiers[_currentSpeedTier])
        {
            _sackObjects[_currentSpeedTier].SetActive(false);
            _currentSpeedTier++;
            _sackObjects[_currentSpeedTier].SetActive(true);
            _sack.transform.localScale = _sackSizeTiers[_currentSpeedTier];
        }
        //otherwise, when the player throws/dunks a present into the goal, if the number of presents is less than the maximum for the previous tier, reduce the weight (increases speed), set corresponding bag to active
        else if (_currentSpeedTier > 0)
        {
            if (presentNo < _maxPresentTiers[_currentSpeedTier - 1])
            {
                _sackObjects[_currentSpeedTier].SetActive(false);
                _currentSpeedTier--;
                _sackObjects[_currentSpeedTier].SetActive(true);
                _sack.transform.localScale = _sackSizeTiers[_currentSpeedTier];
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

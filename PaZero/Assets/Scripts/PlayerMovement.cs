using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Components")]
    private Rigidbody2D _rb;
    public Animator _anim;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Direction Related Variables")]
    private int _facingDirection = 1;
    private bool _facingRight = true;
    private float _input;
    //[SerializeField] SpriteRenderer _sr;

    [Header("Movement Variables")]
    [SerializeField] private float _maxMoveSpeed = 10f;
    [SerializeField] private float _dashStartTime = 0f;

    private float _dashTimer = 0f;
    private float _horizontalDirection = 0f;
    private bool _changingDirection => (_rb.velocity.x > 0f && _horizontalDirection < 0f) || (_rb.velocity.x < 0f && _horizontalDirection > 0f);
    private bool _canMove = true;
    private bool _canDash = true;

    public float MovementSpeed = 4;

    [Header("Ladder Variables")]
    [SerializeField] public float _speed = 8f;
    [SerializeField] public bool _isClimbing;
    [SerializeField] private GameObject arm;
    private float _verticalDirection;
    private bool _isLadder;

    [Header("Jump Variables")]
    [SerializeField] private float _jumpForce = 0f;
    //[SerializeField] private float _airLinearDrag = 2.5f;
    [SerializeField] private float _fallMultiplier = 4f;
    [SerializeField] private float _lowJumpFallMultiplier = 9f;
    [SerializeField] private float _jumpToleranceStartTimer = 0f;
    private float _jumpToleranceTimer = 0f;
    private bool _canJump => Input.GetKeyDown(KeyCode.Space) && _onGround;
    private bool _jump = false;

    [Header("Crouch Variables")]
    [SerializeField] private float _ceilingRaycastLength;
    [SerializeField] private Vector3 _ceilingRaycastOffset;
    public BoxCollider2D _head;
    public bool _isCrouching;
    public bool _cantStand = true;

    [Header("Ground Collision Variables")]
    [SerializeField] private float _groundRaycastLength = 0;
    [SerializeField] private Vector3 _groundRaycastOffset;
    [SerializeField] public CapsuleCollider2D _cc;
    [SerializeField] public bool _onGround;
    private Vector2 _colliderSize;

    [Header("Wall Collision Variables")]
    [SerializeField] private float _wallRaycastLength;
    [SerializeField] private Vector3 _wallRaycastOffset;
    [SerializeField] private float _wallSlideSpeed;
    [SerializeField] private bool _onLeftWall;
    [SerializeField] private bool _onRightWall;
    [SerializeField] private bool _onWallSlide;
    [SerializeField] private float _wallJumpForce;
    [SerializeField] private Vector2 _wallJumpDirection;
    [SerializeField] private bool _wallJumpPerformed = false;
    //private Vector2 _colliderSize;

    [SerializeField] public bool _knockbackPerformed = false;
#pragma warning restore 0649

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        //_sr = GetComponent<SpriteRenderer>();
        _rb.drag = 0f;
        _cc = GetComponent<CapsuleCollider2D>();
        _colliderSize = _cc.size;
        _head = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        AnimationLogic();

        _horizontalDirection = GetInput().x;
        _verticalDirection = GetInput().y;
        if (Input.GetKeyDown(KeyCode.Space) && !_jump && isMovementAllowed())
        {
            _jump = true;
        }
        CrouchLogic();
        // if key down, dash cooldown is over and was grounded once before the last dash...
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashTimer <= 0 && _canDash && isMovementAllowed())
        {
            Dash();
        }
        else
        {
            _dashTimer -= Time.deltaTime;
        }
        if (_isLadder && ((_verticalDirection < 0f && !_onGround) || _verticalDirection > 0f))
        {
            _isClimbing = true;
        }
        if (_isClimbing)
        {
            arm.SetActive(false);
        }
        else
        {
            arm.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        CheckWallSlide();
        _head.isTrigger = _isCrouching;
        //_head.enabled = !_isCrouching;
        if (_isClimbing)
        {
            _rb.gravityScale = 0f;
            _rb.velocity = new Vector2(_rb.velocity.x, _verticalDirection * _speed);
        }
        else
        {
            _rb.gravityScale = _fallMultiplier;
        }
        if (_onGround && _knockbackPerformed)
        {
            _knockbackPerformed = false;
        }
        if (_canMove)
        {
            CheckInput();
            MoveCharacter();
        }
        if (_jump)
        {
            Jump();
            _jump = false;
        }
        else
        {
            FallMultiplier();
        }
        if (_onWallSlide)
        {
            if (_rb.velocity.y < -_wallSlideSpeed)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, -_wallSlideSpeed);
            }
        }
    }

    private void AnimationLogic()
    {
        if (!PauseMenu.isPaused)
        {
            if (_isClimbing)
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    _anim.Play("player_ladder_climb_up");
                }
                else if (Input.GetAxisRaw("Vertical") < 0)
                {
                    _anim.Play("player_ladder_climb_down");
                }
                else
                {
                    _anim.Play("player_ladder_idle");
                }
            }
            else if (_onGround)
            {
                if (Input.GetAxisRaw("Horizontal") != 0 && !_onRightWall && !_onLeftWall)
                {
                    if (MovementSpeed * _facingDirection > 0.01f)
                    {
                        if (_isCrouching)
                        {
                            _anim.Play("player_crouching_forward");
                        }
                        else
                        {
                            _anim.Play("player_walk_forward");
                        }
                    }
                    else if (MovementSpeed * _facingDirection < -0.01f)
                    {
                        if (_isCrouching)
                        {
                            _anim.Play("player_crouching_backwards");
                        }
                        else
                        {
                            _anim.Play("player_walk_backwards");
                        }
                    }
                }
                else
                {
                    if (_isCrouching)
                    {
                        _anim.Play("player_crouch");
                    }
                    else
                    {
                        _anim.Play("player_idle");
                    }
                }
            }
            else
            {
                if (!_onWallSlide)
                {
                    if (_rb.velocity.y > 0)
                    {
                        if (_wallJumpPerformed)
                        {
                            _anim.Play("player_walljump");
                        }
                        else
                        {
                            _anim.Play("player_jump");
                        }
                    }
                    else
                    {
                        _anim.Play("player_fall");
                    }
                }
                else
                {
                    if (_facingDirection > 0.01f)
                    {
                        if (_onRightWall)
                        {
                            _anim.Play("player_slide_front");
                        }
                        else
                        {
                            _anim.Play("player_slide_back");
                        }
                    }
                    else
                    {
                        if (_onLeftWall)
                        {
                            _anim.Play("player_slide_front");
                        }
                        else
                        {
                            _anim.Play("player_slide_back");
                        }
                    }
                }
            }
        }
    }

    private void CrouchLogic()
    {
        if (Input.GetKey(KeyCode.S))
        {
            _isCrouching = true;
            if (_onGround)
            {
                _maxMoveSpeed = 30f;
            }
            else
            {
                _maxMoveSpeed = 10f;
            }
        }
        else
        {
            if (!_cantStand)
            {
                _isCrouching = false;
                _maxMoveSpeed = 10f;
            }
        }
    }


    private bool isMovementAllowed()
    {
        if (PauseMenu.isPaused)
        {
            return false;
        }
        bool answer = DialogueManager.GetInstance().dialogueIsPlaying;
        if(!answer)
        {
            return !PlayerManager._isDead;
        }
        return !answer;
    }

    private static Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void MoveCharacter()
    {
        if (isMovementAllowed())
        {
            MovementSpeed += Input.GetAxisRaw("Horizontal");
        }
        MovementSpeed *= Mathf.Pow(1f - 0.4f, Time.deltaTime * _maxMoveSpeed);
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            MovementSpeed *= Mathf.Pow(1f - 0.4f, Time.deltaTime * _maxMoveSpeed);
        }
        _rb.velocity = new Vector2(MovementSpeed, _rb.velocity.y);

    }

    private void Dash()
    {
        _canDash = false;
        _dashTimer = _dashStartTime;
        MovementSpeed = 10;
        MovementSpeed *= 2.4f * Input.GetAxisRaw("Horizontal");
    }

    private void Jump()
    {
        if (!_onWallSlide && (_onGround || _isClimbing || _jumpToleranceTimer > 0.01f) && !_cantStand)
        {
            _isClimbing = false;
            _wallJumpPerformed = false;
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
        else if (_onWallSlide)
        {
            _wallJumpPerformed = true;
            Vector2 force;

            if (_onLeftWall)
            {
                force = new Vector2(_wallJumpForce * _wallJumpDirection.x, _wallJumpForce * _wallJumpDirection.y);
            }
            else
            {
                force = new Vector2(_wallJumpForce * _wallJumpDirection.x * -1, _wallJumpForce * _wallJumpDirection.y);
            }

            _rb.velocity = Vector2.zero;

            _rb.AddForce(force, ForceMode2D.Impulse);

            MovementSpeed = force.x;
            
            StartCoroutine("StopMove");
        }
    }

    private void FallMultiplier()
    {
        if (_isClimbing)
        {
            _rb.gravityScale = 0f;
        }
        else if (_rb.velocity.y < 0.01)
        {
            _rb.gravityScale = _fallMultiplier;
        }
        else if (_rb.velocity.y > 0.01 && !Input.GetButton("Jump"))
        {
            _rb.gravityScale = _lowJumpFallMultiplier;
        }
        else
        {
            if (_wallJumpPerformed)
            {
                _rb.gravityScale = 3f;
            }
            else if (_knockbackPerformed)
            {
                _rb.gravityScale = 2f;
            }
            else
            {
                _rb.gravityScale = 1.3f;
            }
        }

    }

    private void CheckCollisions()
    {
        _onGround = Physics2D.Raycast(transform.position + _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(transform.position - _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer);

        _onLeftWall = Physics2D.Raycast(transform.position + _wallRaycastOffset, Vector2.left, _wallRaycastLength, _groundLayer) ||
                        Physics2D.Raycast(transform.position - _wallRaycastOffset, Vector2.left, _wallRaycastLength, _groundLayer);
        _onRightWall = Physics2D.Raycast(transform.position + _wallRaycastOffset, Vector2.right, _wallRaycastLength, _groundLayer) ||
                        Physics2D.Raycast(transform.position - _wallRaycastOffset, Vector2.right, _wallRaycastLength, _groundLayer);

        _cantStand = Physics2D.Raycast(transform.position + _ceilingRaycastOffset, Vector2.up, _ceilingRaycastLength, _groundLayer) ||
                    Physics2D.Raycast(transform.position - _ceilingRaycastOffset, Vector2.up, _ceilingRaycastLength, _groundLayer);

        //Gizmos.DrawLine(transform.position + _ceilingRaycastOffset, transform.position + _ceilingRaycastOffset + Vector3.up * _ceilingRaycastLength);
        //Gizmos.DrawLine(transform.position - _ceilingRaycastOffset, transform.position - _ceilingRaycastOffset + Vector3.up * _ceilingRaycastLength);

        if (_onGround || _onLeftWall || _onRightWall)
        {
            _canDash = true;
        }

        if (!_onGround)
        {
            _jumpToleranceTimer -= Time.deltaTime;
        }
        else
        {
            _jumpToleranceTimer = _jumpToleranceStartTimer;
        }
    }

    private void CheckWallSlide()
    {
        if ((_onLeftWall || _onRightWall) && !_onGround && !_isClimbing) // && GetInput().x != 0
        {
            if (_rb.velocity.y < 0)
            {
                //do animation stuff here...
            }
            _onWallSlide = true;
        }
        else
        {
            _onWallSlide = false;
        }
    }

    private void CheckInput()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mouseWorldPosition.x - transform.position.x);

        /*
        if((mouseWorldPosition.x - transform.position.x) < 0 && _facingRight)
        {
            _facingRight = !_facingRight;
            Flip();
        }
        else
        {
            _facingRight = true;
            Flip();
        }*/


        _input = Input.GetAxisRaw("Horizontal");
        if (((mouseWorldPosition.x - transform.position.x) < 0 && _facingRight) || ((mouseWorldPosition.x - transform.position.x) > 0 && !_facingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        _facingDirection *= -1;
        _facingRight = !_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    IEnumerator StopMove()
    {
        // remove controle do personagem
        _canMove = false;
        //transform.localScale = transform.localScale.x == 1 ? new Vector2(-1, 1) : Vector2.one;

        yield return new WaitForSeconds(.24f);

        // normaliza lado do transform
        transform.localScale = Vector2.one;
        // devolve o controle para o personagem
        _canMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + _groundRaycastOffset, transform.position + _groundRaycastOffset + Vector3.down * _groundRaycastLength);
        Gizmos.DrawLine(transform.position - _groundRaycastOffset, transform.position - _groundRaycastOffset + Vector3.down * _groundRaycastLength);
        Gizmos.color = Color.yellow;
        // left side
        Gizmos.DrawLine(transform.position + _wallRaycastOffset, transform.position + _wallRaycastOffset + Vector3.left * _wallRaycastLength);
        Gizmos.DrawLine(transform.position - _wallRaycastOffset, transform.position - _wallRaycastOffset + Vector3.left * _wallRaycastLength);
        // right side
        Gizmos.DrawLine(transform.position + _wallRaycastOffset, transform.position + _wallRaycastOffset + Vector3.right * _wallRaycastLength);
        Gizmos.DrawLine(transform.position - _wallRaycastOffset, transform.position - _wallRaycastOffset + Vector3.right * _wallRaycastLength);
        // ceiling check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + _ceilingRaycastOffset, transform.position + _ceilingRaycastOffset + Vector3.up * _ceilingRaycastLength);
        Gizmos.DrawLine(transform.position - _ceilingRaycastOffset, transform.position - _ceilingRaycastOffset + Vector3.up * _ceilingRaycastLength);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isLadder = false;
            _isClimbing = false;
        }
    }
}

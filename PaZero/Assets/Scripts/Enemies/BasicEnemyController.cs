using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
#pragma warning disable 0649
    // will use a state machine with a bunch of predefined states
    private enum State
    {
        Moving,
        Knockback,
        Dead
    }

    private State _currentState;

    [SerializeField] private float
        _groundCheckDistance,
        _wallCheckDistance,
        _movementSpeed,
        _maxHealth,
        _knockbackDuration;
    private float
        _currentHealth,
        _knockbackStartTime,
        _knockbackMultiplier;
    [SerializeField] private Transform 
        _groundCheck,
        _wallCheck;
    [SerializeField] LayerMask 
        _groundLayer;
    [SerializeField] Vector2
        _knockbackSpeed;

    private GameObject _alive;
    private Rigidbody2D _aliveRb;
    private Vector2 _movement;
    private Animator _aliveAnim;

    private int 
        _facingDirection,
        _damageDirection;

    private bool _groundDetected;
    private bool _wallDetected;
#pragma warning restore 0649

    private void Start()
    {
        _alive = transform.Find("Alive").gameObject;
        _aliveRb = _alive.GetComponent<Rigidbody2D>();
        //_aliveAnim = alive.GetComponent<Animator>();

        _currentHealth = _maxHealth;

        _facingDirection = 1;
    }


    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //--Moving STATE--------------------
    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        _groundDetected = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundCheckDistance, _groundLayer);
        _wallDetected = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckDistance, _groundLayer) ||
                        Physics2D.Raycast(_wallCheck.position, -transform.right, _wallCheckDistance, _groundLayer);

        if(!_groundDetected || _wallDetected)
        {
            Flip();
        }
        else
        {

            _movementSpeed += _facingDirection;
            _movementSpeed *= Mathf.Pow(1f - 0.8f, Time.deltaTime * 18f);
            _aliveRb.velocity = new Vector2(_movementSpeed, _aliveRb.velocity.y);

            //_movement.Set(_movementSpeed * _facingDirection, _aliveRb.velocity.y);
            //_aliveRb.velocity = _movement;
        }
    }

    private void ExitMovingState()
    {

    }

    //--KNOCKBACK-----------------
    private void EnterKnockbackState()
    {
        _knockbackStartTime = Time.time;
        _movement.Set(_knockbackSpeed.x * _knockbackMultiplier * _damageDirection / 8, _knockbackSpeed.y * _knockbackMultiplier / 8);
        _aliveRb.velocity = _movement;
        //_aliveAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if(Time.time >= _knockbackStartTime + _knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        //_aliveAnim.SetBool("Knockback", false);
    }

    //--DEAD STATE----------
    private void EnterDeadState()
    {
        // Spawn chunks and blood
        Destroy(this.gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //--OTHER FUNCTIONS--------------------

    public void TakeDamage(float[] attackDetails)
    {
        // damage on first index
        _currentHealth -= attackDetails[0];
        // player x position on second index
        if(attackDetails[1] > _alive.transform.position.x)
        {
            _damageDirection = -1;
        }
        else
        {
            _damageDirection = 1;
        }

        // Hit particle
        if(_currentHealth > 0.0f)
        {
            _knockbackMultiplier = attackDetails[0];
            SwitchState(State.Knockback);
            _currentHealth -= attackDetails[0];
        } else if(_currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void Flip()
    {
        _facingDirection *= -1;
        _alive.transform.Rotate(0f, 180f, 0f);
    }

    private void SwitchState(State state)
    {
        switch (_currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        _currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_groundCheck.position, new Vector2(_groundCheck.position.x, _groundCheck.position.y - _groundCheckDistance));
        Gizmos.DrawLine(_wallCheck.position, new Vector2(_wallCheck.position.x + _wallCheckDistance, _wallCheck.position.y));
        Gizmos.DrawLine(_wallCheck.position, new Vector2(_wallCheck.position.x - _wallCheckDistance, _wallCheck.position.y));
    }
}

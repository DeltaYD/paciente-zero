using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine _stateMachine;

    public D_Entity _entityData;

    public int _facingDirection { get; private set; }

    public Rigidbody2D _rb { get; private set; }
    public Animator _anim { get; private set; }
    public GameObject _aliveObj{ get; private set; }
    public int lastDamageDirection { get; private set; }

    private float _speed = 0f;
    private float defaultRange = 7.76f;

#pragma warning disable 0649
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Transform _ledgeCheck;
    [SerializeField] private Transform _playerCheck;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private BoxCollider2D _contactCheck;

    [Tooltip("Flash effect")]
    [SerializeField] private FlashDamage _flashEffect;

    [Tooltip("Health bar thing")]
    [SerializeField] private LoadBar _bar;
#pragma warning restore 0649

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 _velocityWorkspace;

    protected bool isStunned;
    protected bool isDead;

    public virtual void Start()
    {
        isDead = false;
        currentStunResistance = _entityData.stunResistance;
        currentHealth = _entityData.maxHealth;
        _facingDirection = 1;
        _aliveObj = transform.Find("Alive").gameObject;
        _rb = _aliveObj.GetComponent<Rigidbody2D>();
        _anim = _aliveObj.GetComponent<Animator>();
        _stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        _stateMachine.currentState.LogicUpdate();

        if(Time.time >= lastDamageTime + _entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }

        if(_bar != null)
        {
            _bar.SetBar(currentHealth / _entityData.maxHealth);
        }
    }

    public virtual void FixedUpdate()
    {
        _stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        //_velocityWorkspace.Set(_facingDirection * Mathf.Pow(1f - 0.8f, Time.deltaTime * 18f), _rb.velocity.y);
        //_rb.velocity = _velocityWorkspace;

        _speed += _facingDirection;
        _speed *= Mathf.Pow(1f - 0.8f, Time.deltaTime * 18f);
        _rb.velocity = new Vector2(_speed * velocity, _rb.velocity.y);
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        _velocityWorkspace.Set(angle.x * velocity * direction, angle.y * velocity);
        _rb.velocity = _velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(_wallCheck.position, _aliveObj.transform.right, _entityData.wallCheckDistance, _entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(_ledgeCheck.position, Vector2.down, _entityData.ledgeCheckDistance, _entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, _entityData.groundCheckRadius, _entityData.whatIsGround);
    }

    public virtual bool CheckPlayerInRange()
    {
        //instead, call fieldofview script
        //return Physics2D.Raycast(_playerCheck.position, _aliveObj.transform.right, _entityData.detectDistance, _entityData.whatIsPlayer);
        _aliveObj.GetComponentInChildren<FieldOfView>().radius = defaultRange;
        return _aliveObj.GetComponentInChildren<FieldOfView>().canSeePlayer;
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(_playerCheck.position, _aliveObj.transform.right, _entityData.closeRangeActionDistance, _entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInShootingRange()
    {
        _aliveObj.GetComponentInChildren<FieldOfView>().radius = 2*defaultRange;
        return _aliveObj.GetComponentInChildren<FieldOfView>().canTrackPlayer;
    }

    public virtual bool CheckIfFacingPlayer()
    {
        if((PlayerCurrentPosition().x > transform.position.x && _facingDirection == -1) ||
           (PlayerCurrentPosition().x < transform.position.x && _facingDirection == 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual Vector3 PlayerLastSeenPosition()
    {
        return _aliveObj.GetComponentInChildren<FieldOfView>().lastSeenPos;
    }

    public virtual Vector3 PlayerCurrentPosition()
    {
        return _aliveObj.GetComponentInChildren<FieldOfView>().target.position;
    }

    public virtual Vector2 PlayerDirection()
    {
        return _aliveObj.GetComponentInChildren<FieldOfView>().directionToTarget;
    }

    public virtual void DamageHop(float velocity)
    {
        _velocityWorkspace.Set(_rb.velocity.x, velocity);
        _rb.velocity = _velocityWorkspace;
    }

    public virtual void DamageFlash()
    {
        _flashEffect.Flash();
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = _entityData.stunResistance;
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time;

        currentHealth -= attackDetails.damageAmount;
        currentStunResistance -= attackDetails.stunDamageAmount;

        DamageHop(_entityData.damageHopSpeed);
        DamageFlash();

        if (_entityData.hitParticle != null)
        {
            Instantiate(_entityData.hitParticle, _aliveObj.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
        }
        if (attackDetails.position.x > _aliveObj.transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }

        if(currentStunResistance <= 0)
        {
            isStunned = true;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public virtual void ContactDamage()
    {
        if (_contactCheck.CompareTag("Player"))
        {
            _contactCheck.GetComponent<PlayerManager>().TakeDamage(10f);
            _contactCheck.GetComponent<PlayerCombat>().ContactKnockback();
        }
    }

    public virtual void Flip()
    {
        _facingDirection *= -1;
        _aliveObj.transform.Rotate(0f, 180f, 0f);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_wallCheck.position, _wallCheck.position + (Vector3)(Vector2.right * _facingDirection * _entityData.wallCheckDistance));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_ledgeCheck.position, _ledgeCheck.position + (Vector3)(Vector3.down * _entityData.ledgeCheckDistance));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_playerCheck.position, _playerCheck.position + (Vector3)(Vector2.right * _facingDirection * _entityData.detectDistance));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator _animator;
    private Rigidbody2D _rb;
    private PlayerManager _player;
    private PlayerMovement _pm;
#pragma warning disable 0649
    [SerializeField] private AudioManager _am;
#pragma warning restore 0649
    public MousePosition2D _mp;

    public Transform _attackPoint;
    public float _attackRange = 0.5f;
    public LayerMask _enemyLayers;
    public LayerMask _leverLayers;

    public float _attackRate = 2f;
    float _nextAttackTime = 0f;

    public float _power = 0f;
    public float _maxpower = 10f;
    public float _chargeSpeed = 3f;
    public float thrust;
    AttackDetails _attackDetails;

    public bool _charging = false;

    private bool _buttonHeldDown;

    [Header("Baton Sprite Components")]
    private bool batonHasEnergy = true;

#pragma warning disable 0649
    [SerializeField] private SpriteRenderer component;
    [SerializeField] private Sprite batonCharged;
    [SerializeField] private Sprite batonUncharged;
    [SerializeField] private GameObject pointLight;
    [SerializeField] private Material lit;
    [SerializeField] private Material unlit;
    [SerializeField] private float stunDamageAmount;
#pragma warning restore 0649

    private void Start()
    {
        stunDamageAmount = 3 / _maxpower;
        _player = GetComponent<PlayerManager>();
        _rb = GetComponent<Rigidbody2D>();
        _pm = GetComponent<PlayerMovement>();
        _mp = GetComponentInChildren<MousePosition2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if not on cooldown and player can attack...
        if(Time.time >= _nextAttackTime && _player._hasEnergyBaton && !DialogueManager.GetInstance().dialogueIsPlaying && !PauseMenu.isPaused && !PlayerManager._isDead)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _charging = true;
                // while pressed down, charge attack
                if(_power <= _maxpower)
                {
                    _power += Time.deltaTime * _chargeSpeed;
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _charging = false;
                if(_power < 2)
                {
                    _power = 2f;
                }
                // when released, attack with the power calculated
                Debug.Log(_power);
                _am.Play("BatonSwing");
                Attack(_power);
                Knockback(_power);
                Activate();

                // reset to the original power level
                _power = 0;

                // put it on cooldown
                _nextAttackTime = Time.time + 1f / _attackRate;
            }
        }
    }

    private void FixedUpdate()
    {
        if(_mp == null)
        {
            _mp = GetComponentInChildren<MousePosition2D>();
        }
        else
        {
            if (_charging)
            {
                _mp.ChargeUp();
            }
            else
            {
                _mp.Release();
            }
        }
    }

    void Knockback(float power)
    {
        Vector2 difference = _attackPoint.position - transform.position;
        difference = -difference.normalized * power;
        _rb.velocity = Vector2.zero;
        if (!_pm._onGround)
        {
            _rb.AddForce(difference, ForceMode2D.Impulse);
        }
        //_rb.AddForce(new Vector2(60,60), ForceMode2D.Impulse);
        _pm.MovementSpeed = difference.x*2;
        _pm._knockbackPerformed = true;
    }

    public void ContactKnockback()
    {
        //_pm.
    }

    void Attack(float power)
    {
        if (!batonHasEnergy)
        {
            power /= 2;
        }
        _attackDetails.damageAmount = power;
        _attackDetails.position = transform.position;
        _attackDetails.stunDamageAmount = stunDamageAmount * power;
        //_animator.SetTrigger("Attack");

        //Detect enemies in range
        Collider2D[] _hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayers);

        foreach (Collider2D _enemy in _hitEnemies)
        {
            _am.Play("PlayerHitsEnemySFX");
            RoomManager.Instance.CameraShake(3f, .2f);
            _enemy.GetComponentInParent<Entity>().Damage(_attackDetails);
        }
    }

    void Activate()
    {
        Collider2D[] _leverHit = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _leverLayers);

        foreach(Collider2D _lever in _leverHit)

        _lever.GetComponent<Lever>().Activate(batonHasEnergy);

    }

    public void ChangeBatonSprite()
    {
        batonHasEnergy = !batonHasEnergy;
        if (batonHasEnergy)
        {
            component.sprite = batonCharged;
            pointLight.SetActive(true);
            component.material = lit;
        }
        else
        {
            component.sprite = batonUncharged;
            pointLight.SetActive(false);
            component.material = unlit;
        }
    }

    void OnDrawGizmos()
    {
        if (_attackPoint == null) return;

        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Entity
{
    public GameObject firePoint;
    
    public E1_IdleState idleState { get; private set; }
    public E1_MoveState moveState { get; private set; }
    public E1_PlayerDetectedState playerDetectedState { get; private set; }
    public E1_ChargeState chargeState { get; private set; }
    public E1_ShootingState shootingState { get; private set; }
    public E1_HuntState huntState { get; private set; }
    public E1_StunState stunState { get; private set; }
    public E1_DeadState deadState { get; private set; }

#pragma warning disable 0649
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_ChargeState chargeStateData;
    [SerializeField] private D_ShootingState shootingStateData;
    [SerializeField] private D_HuntState huntStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;
#pragma warning restore 0649

    public override void Start()
    {
        base.Start();

        moveState = new E1_MoveState(this, _stateMachine, "move", moveStateData, this);
        idleState = new E1_IdleState(this, _stateMachine, "idle", idleStateData, this);
        playerDetectedState = new E1_PlayerDetectedState(this, _stateMachine, "playerDetected", playerDetectedData, this);
        chargeState = new E1_ChargeState(this, _stateMachine, "charge", chargeStateData, this);
        shootingState = new E1_ShootingState(this, _stateMachine, "shooting", shootingStateData, this, firePoint);
        huntState = new E1_HuntState(this, _stateMachine, "hunting", huntStateData, this);
        stunState = new E1_StunState(this, _stateMachine, "stun", stunStateData, this);
        deadState = new E1_DeadState(this, _stateMachine, "dead", deadStateData, this);

        _stateMachine.Initialize(moveState);

    }

    public override void Damage(AttackDetails attackDetails)
    {
        base.Damage(attackDetails);

        if (isDead)
        {
            _stateMachine.ChangeState(deadState);
        }
        else if (isStunned)
        {
            _stateMachine.ChangeState(stunState);
        }

    }
}

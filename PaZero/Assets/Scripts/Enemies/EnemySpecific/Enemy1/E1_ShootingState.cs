using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ShootingState : ShootingState
{

    private Enemy1 enemy;
    private GameObject firePoint;
    private float shootTimer;

    public E1_ShootingState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ShootingState stateData, Enemy1 enemy, GameObject firePoint) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
        this.firePoint = firePoint;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        
    }

    public override void Enter()
    {
        base.Enter();

        shootTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if(entity.PlayerDirection().x * entity._facingDirection < 0)
        {
            entity.Flip();
        }
        
        if (isPlayerInShootingRange)
        {
            if (currentAmmo != 0)
            {
                shootTimer += Time.deltaTime;
                if (shootTimer > stateData.fireRate)
                {
                    Shoot(firePoint);
                    shootTimer = 0;
                }
            }
            else
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer > stateData.reloadTime)
                {
                    currentAmmo = stateData.maxAmmo;
                    reloadTimer = 0;
                }
            }
        }
        else
        {
            if (performPlayerMissingAction)
            {
                stateMachine.ChangeState(enemy.huntState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : State
{
    // contains: fire accuracy, fire rate, max ammo, sight range and shooting position
    protected D_ShootingState stateData;
    protected bool isPlayerInShootingRange;
    protected int currentAmmo;
    protected float reloadTimer;
    protected float huntCounter, huntTimer;
    protected bool performPlayerMissingAction;

    public ShootingState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ShootingState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        // if player is shootable (if he's in the fov)
        isPlayerInShootingRange = entity.CheckPlayerInShootingRange();
    }

    public override void Enter()
    {
        base.Enter();

        performPlayerMissingAction = false;

        reloadTimer = 0;
        currentAmmo = stateData.maxAmmo;
        entity.SetVelocity(0f);

        huntCounter = 0;
        huntTimer = stateData.beginHuntActionTimer;
    }

    public override void Exit()
    {
        base.Exit();
    }

    // this is pretty much the update function
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isPlayerInShootingRange)
        {
            huntCounter += Time.deltaTime;
            if (huntCounter > huntTimer)
            {
                performPlayerMissingAction = true;
            }
        }
        else
        {
            huntCounter = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void Shoot(GameObject firePoint)
    {
        Object.Instantiate(stateData.bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        currentAmmo--;
    }
}

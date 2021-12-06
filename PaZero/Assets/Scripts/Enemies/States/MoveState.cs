using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{

    protected D_MoveState stateData;

    private float curSpeed;

    protected bool isDetectingWall;
    protected bool isDetectingLedge;
    protected bool isPlayerInRange;

    public MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();
        //entity.SetVelocity(stateData.movementSpeed);
        curSpeed = stateData.movementSpeed;
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //entity.SetVelocity(0);

        if (!isDetectingLedge || isDetectingWall || isPlayerInRange)
        {
            entity.SetVelocity(0);
        }
        else
        {
            entity.SetVelocity(curSpeed);
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isDetectingLedge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
        isPlayerInRange = entity.CheckPlayerInRange();
    }
}

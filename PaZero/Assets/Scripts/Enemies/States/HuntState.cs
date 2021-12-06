using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntState : State
{
    protected D_HuntState stateData;
    protected bool isPlayerInRange;
    protected bool performLongRangeAction;
    protected Vector2 lastSeenPosition;
    protected float curSpeed;

    protected bool isDetectingWall;
    protected bool isDetectingLedge;


    public HuntState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_HuntState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isDetectingLedge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();

        isPlayerInRange = entity.CheckPlayerInRange();
        if (isPlayerInRange)
        {
            performLongRangeAction = true;
        }
    }

    public override void Enter()
    {
        base.Enter();
        performLongRangeAction = false;
        curSpeed = stateData.movementSpeed;

        lastSeenPosition = entity.PlayerLastSeenPosition();
        Debug.Log("Hunt state! Last seen at:" + lastSeenPosition);
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

        if (!isDetectingLedge || isDetectingWall || isPlayerInRange)
        {
            entity.SetVelocity(0); // seek place to jump at here
        }
        else
        {
            entity.SetVelocity(curSpeed);
        }
    }
}

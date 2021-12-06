using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : State
{
    protected D_PlayerDetected stateData;

    protected bool isPlayerInRange;
    //protected bool performedShortRangeAction;
    protected bool performLongRangeAction;
    protected bool performPatrol;

    protected float detectTimer;
    protected float undetectedTimer;
    protected float detectCounter;
    protected float undetectedCounter;

    public PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        
        isPlayerInRange = entity.CheckPlayerInRange();
        if (isPlayerInRange)
        {
            undetectedCounter = 0;
            detectCounter += Time.deltaTime;

            if (detectCounter > detectTimer)
            {
                performLongRangeAction = true;
                Debug.Log("Hands up!");
            }
        }
        else
        {
            detectCounter = 0;
            undetectedCounter += Time.deltaTime;

            if (undetectedCounter > undetectedTimer)
            {
                performPatrol = true;
                Debug.Log("Guess it was the wind...");
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        detectCounter = 0;
        undetectedCounter = 0;
        detectTimer = stateData.detectTimer;
        undetectedTimer = stateData.undetectedTimer;
        performLongRangeAction = false;
        performPatrol = false;

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
        entity.SetVelocity(0f);
        base.PhysicsUpdate();
    }
}

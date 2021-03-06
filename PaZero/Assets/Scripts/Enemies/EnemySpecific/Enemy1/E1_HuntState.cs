using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_HuntState : HuntState
{
    private Enemy1 enemy;

    public E1_HuntState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_HuntState stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy.shootingState);
        } else if (isDetectingWall || !isDetectingLedge)
        {
            enemy.Flip();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

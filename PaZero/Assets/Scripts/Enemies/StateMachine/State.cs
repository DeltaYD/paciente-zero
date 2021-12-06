using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    // private, but any class inheriting it can use it
    protected FiniteStateMachine stateMachine;
    protected Entity entity;

    protected float startTime;

    protected string animBoolName;

    public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // can be redefined in derived classes
    public virtual void Enter()
    {
        startTime = Time.time;
        entity._anim.SetBool(animBoolName, true);
        DoChecks();
    }

    public virtual void Exit()
    {
        entity._anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {
        DoChecks();
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void DoChecks()
    {

    }

}

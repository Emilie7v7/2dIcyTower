using System.Collections;
using System.Collections.Generic;
using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

public class EntityPlayerDetectedState : EntityState
{

    protected bool PerformLongRangeAction;
    protected bool IsPlayerInLineOfSight;

    protected Movement Movement => _movement != null ? _movement : Core.GetCoreComponent<Movement>();
    private Movement _movement;
    
    protected CollisionSenses CollisionSenses => _collisionSenses != null ? _collisionSenses : Core.GetCoreComponent<CollisionSenses>();
    private CollisionSenses _collisionSenses;
    
    public EntityPlayerDetectedState(Entity entity, EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSenses)
        {
            IsPlayerInLineOfSight = CollisionSenses.IsPlayerInLineOfSight();
        }
    }

    public override void Enter()
    {
        base.Enter();
        
        Movement?.SetZeroVelocity();
        
        PerformLongRangeAction = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= StartTime + EntityData.longRangeAttackCooldown)
        {
            PerformLongRangeAction = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using _Scripts.CoreSystem;
using _Scripts.PlayerComponent;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    
    //Input
    protected int XInput;
    protected int YInput;
    
    private bool jumpInput;
    
    //Checks
    private bool isGrounded;
    
    protected PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
    {
    }

    protected Movement Movement { get => movement != null ? movement : Core.GetCoreComponent(ref movement); }
    private Movement movement;
    
    protected CollisionSenses CollisionSenses { get => collisionSenses != null ? collisionSenses : Core.GetCoreComponent(ref collisionSenses); }
    private CollisionSenses collisionSenses;

    public override void DoChecks()
    {
        base.DoChecks();

        if (collisionSenses)
        {
            isGrounded = CollisionSenses.Ground;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        XInput = Player.InputHandler.NormInputX;
        YInput = Player.InputHandler.NormInputY;
    }
}

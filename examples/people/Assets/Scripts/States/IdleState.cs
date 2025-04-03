using UnityEngine;


public class IdleState : UnitState
{
    public IdleState(UnitScript unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log($"{unit.characterName} entered idle state");
    }

    public override void Exit()
    {
        Debug.Log($"{unit.characterName} left idle state");
    }

    public override void Update()
    {
        // Add any idle-specific update logic here
    }
}

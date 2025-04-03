using UnityEngine;


public class WalkingState : UnitState
{
    public WalkingState(UnitScript unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log($"{unit.characterName} started walking");
    }

    public override void Exit()
    {
        Debug.Log($"{unit.characterName} stopped walking");
    }

    public override void Update()
    {
        // Check if we've reached our destination
        if (!unit.nma.pathPending && unit.nma.remainingDistance <= unit.nma.stoppingDistance)
        {
            unit.ChangeState(new IdleState(unit));
        }
    }
}

using UnityEngine;


public class ActionActiveState : UnitState
{
    public ActionActiveState(UnitScript unit) : base(unit) { }

    public override void Enter()
    {
        Debug.Log($"{unit.characterName} started action");
    }

    public override void Exit()
    {
        Debug.Log($"{unit.characterName} ended action");
    }

    public override void Update()
    {
        // Add any action-specific update logic here
    }
}

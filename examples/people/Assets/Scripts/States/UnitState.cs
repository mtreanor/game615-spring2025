using UnityEngine;


public abstract class UnitState
{
    protected UnitScript unit;

    public UnitState(UnitScript unit)
    {
        this.unit = unit;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitScript : MonoBehaviour
{
    public NavMeshAgent nma;

    // The below are all of the stuff that populates the UI (and makes the units
    // individual).
    public string characterName;

    public Renderer bodyRenderer;

    public Color unselectedColor;
    public Color selectedColor;

    private UnitState currentState;

    private void OnEnable()
    {
        // Initialize state
        ChangeState(new IdleState(this));
    }

    private void OnDisable()
    {
        // Clean up state
        ChangeState(new IdleState(this));
    }

    // Start is called before the first frame update
    void Start()
    {
        nma = gameObject.GetComponent<NavMeshAgent>();
        unselectedColor = bodyRenderer.material.color;
        GameManager.instance.units.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.Update();
    }

    public void GoToPoint(Vector3 point)
    {
        nma.SetDestination(point);
        ChangeState(new WalkingState(this));
    }

    public void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;

        // Handle state exit
        currentState?.Exit();

        // Change to new state
        currentState = newState;

        // Handle state enter
        currentState?.Enter();
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }

    private void OnDestroy()
    {
        GameManager.instance.units.Remove(this);
    }
}

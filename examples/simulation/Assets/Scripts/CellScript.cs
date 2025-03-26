using UnityEngine;
using System.Collections.Generic;

// Represents a single cell in the simulation grid
// Handles the cell's state and visual representation
public class CellScript : MonoBehaviour
{
    // References to visual components
    [SerializeField] GameObject selectionPlane;
    [SerializeField] GameObject heightCube;
    private Material heightCubeMaterial;

    Color defaultColor;

    // Cell state with property to update visuals when changed
    public CellState State = new CellState();


    void Start()
    {
        // Cache the material for performance and initialize visuals
        heightCubeMaterial = heightCube.GetComponentInChildren<Renderer>().material;
        defaultColor = heightCubeMaterial.color;
        UpdateVisuals();
    }

    void Update()
    {
        
    }

    void ResetCellState() {
        State.height = 0;
        UpdateVisuals();
    }   

    public void Hover() {
        selectionPlane.SetActive(true);
        // Update the selection plane's position to match the state's height
        float height = transform.position.y + State.height + 0.1f;
        selectionPlane.transform.position = new Vector3(selectionPlane.transform.position.x, height, selectionPlane.transform.position.z);
    }

    public void Unhover() {
        selectionPlane.SetActive(false);
    }

    public void Clicked() {
        
    }

    public void RightClicked() {
        
    }   

    // Calculates the next state of this cell for the simulation
    public CellState GenerateNextSimulationStep()
    {
        // Create a copy of the current state to modify
        CellState nextState = this.State.Clone();
        // This is just an example
        ApplyMountainSmoothing(nextState);

        return nextState;
    }

    void ApplyMountainSmoothing(CellState cellState) {
        // Get all neighboring cells (excluding the current cell)
        List<CellScript> neighbors = GridManager.Instance.GetNeighbors(this, true);
        
        // Calculate the average height of all neighboring cells
        float totalHeight = 0;
        foreach (CellScript neighbor in neighbors) {
            totalHeight += neighbor.State.height;
        }
        
        // Set the next height to be the average of all neighbors
        // This creates a smoothing/diffusion effect across the grid
        cellState.height = totalHeight / neighbors.Count;
    }

    // Updates the visual representation of the cell based on its state
    public void UpdateVisuals()
    {
        // Adjust the height cube to match the cell's height value
        if (heightCube != null) {
            heightCube.transform.localScale = new Vector3(1, State.height, 1);
        }

        if (State.pathStateVisuals == "start") {
            heightCubeMaterial.color = Color.white;
        } else if (State.pathStateVisuals == "end") {
            heightCubeMaterial.color = Color.black;
        } else if (State.pathStateVisuals == "open") {
            heightCubeMaterial.color = Color.green;
        } else if (State.pathStateVisuals == "closed") {
            heightCubeMaterial.color = Color.red;
        } else if (State.pathStateVisuals == "path") {
            heightCubeMaterial.color = Color.blue;
        } else {
            heightCubeMaterial.color = defaultColor;
        }
    }
}

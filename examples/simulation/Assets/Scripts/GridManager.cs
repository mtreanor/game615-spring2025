using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
// Manages the grid of cells for the simulation.
// Handles grid creation, cell updates, and provides utility functions for cell access.
public class GridManager : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static GridManager Instance { get; private set; }

    // Reference to the prefab used to create each cell
    [SerializeField]
    GameObject cellPrefab;

    // Grid dimensions
    [SerializeField]
    int gridW = 10;
    [SerializeField]
    int gridH = 5;

    // Materials for visual feedback when hovering over cells
    [SerializeField]
    Material hoverMaterial;
    [SerializeField]
    Material defaultMaterial;

    Coroutine pathfindingCoroutine;
    float pathfindingSpeed = 0.05f;

    // Cell size parameters
    float cellWidth = 1;
    float cellHeight = 1;
    float spacing = 0.0f;

    // Maximum height value for cells (used for normalization)
    public float maxHeight = 5;

    // Timing control for simulation updates
    float nextSimulationStepTimer = 0;
    float nextSimulationStepRate = 0.5f;

    // The 2D array that stores all cell references
    public CellScript[,] grid;

    public CellScript startCell;
    public CellScript endCell;
    
    // Tracks which cell the mouse is currently hovering over
    public CellScript currentHoverCell;

    // Initializes the singleton instance
    private void Awake()
    {
        // Standard singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        Instance = this;
    }

    // Cleans up the singleton reference when destroyed
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Called once when the script is enabled
    void Start()
    {
        GenerateGrid();
    }

    // Called every frame
    void Update()
    {
        // Handle simulation timing
        nextSimulationStepTimer -= Time.deltaTime;
        if (nextSimulationStepTimer < 0 && Input.GetKey(KeyCode.Space)) {
            SimulationStep();
            nextSimulationStepTimer = nextSimulationStepRate;
        }

        // Handle mouse hover detection
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("cell"))) {
            // Get the cell that was hit
            CellScript cs = hit.collider.gameObject.GetComponentInParent<CellScript>();
            Vector2Int gridPosition = new Vector2Int(cs.State.x, cs.State.y);
            
            // Reset previous hover cell's material if we've moved to a new cell
            if (currentHoverCell != null && currentHoverCell != grid[gridPosition.x, gridPosition.y]) {
                currentHoverCell.gameObject.GetComponentInChildren<Renderer>().material = defaultMaterial;
                currentHoverCell.Unhover();
            }
            
            // Update current hover cell and change its material
            currentHoverCell = grid[gridPosition.x, gridPosition.y];
            currentHoverCell.Hover();

            if (Input.GetMouseButtonDown(0)) {
                if (pathfindingCoroutine != null) {
                    StopCoroutine(pathfindingCoroutine);
                    startCell = null;
                    endCell = null;
                    GenerateGrid();
                }
                currentHoverCell.Clicked();
                if (startCell == null) {
                    // Reset all cells to default state
                    foreach (var cell in grid) {
                        cell.State.pathStateVisuals = "default";
                        cell.UpdateVisuals();
                    }
                    startCell = currentHoverCell;
                    startCell.State.pathStateVisuals = "start";
                    startCell.UpdateVisuals();
                } else {
                    endCell = currentHoverCell;
                    endCell.State.pathStateVisuals = "end";
                    endCell.UpdateVisuals();
                    pathfindingCoroutine = StartCoroutine(AStarPath());
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                currentHoverCell.RightClicked();
            }
        }
    }

    IEnumerator AStarPath() {
        // A* Pathfinding algorithm implementation
        List<CellScript> openSet = new List<CellScript>();
        HashSet<CellScript> closedSet = new HashSet<CellScript>();
        openSet.Add(startCell); // Start from the first cell
        
        // Visualize the starting and ending cells
        startCell.State.pathStateVisuals = "start";
        startCell.UpdateVisuals();
        endCell.State.pathStateVisuals = "end";
        endCell.UpdateVisuals();

        Dictionary<CellScript, float> gScore = new Dictionary<CellScript, float>();
        Dictionary<CellScript, float> fScore = new Dictionary<CellScript, float>();
        Dictionary<CellScript, CellScript> cameFrom = new Dictionary<CellScript, CellScript>();

        foreach (var cell in grid) {
            gScore[cell] = float.MaxValue; // Cost from start to the cell
            fScore[cell] = float.MaxValue; // Total cost from start to goal through the cell
        }
        gScore[startCell] = 0;
        fScore[startCell] = Heuristic(startCell, endCell); // Heuristic cost from start to goal

        while (openSet.Count > 0) {
            CellScript current = GetLowestFScoreCell(openSet, fScore);
            if (current == endCell) {
                // Reconstruct and visualize the final path
                List<CellScript> path = ReconstructPath(cameFrom, current);
                foreach (CellScript cell in path) {
                    if (cell != startCell && cell != endCell) {
                        cell.State.pathStateVisuals = "path";
                        cell.UpdateVisuals();
                        yield return new WaitForSeconds(pathfindingSpeed);
                    }
                }
                
                // Reset the selection for the next pathfinding
                startCell = null;
                endCell = null;

                pathfindingCoroutine = null;
                
                yield break;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current != startCell && current != endCell) {
                current.State.pathStateVisuals = "closed";
                current.UpdateVisuals();
            }

            foreach (var neighbor in GetNeighbors(current)) {
                if (closedSet.Contains(neighbor)) continue; // Ignore already evaluated neighbors

                float tentativeGScore = gScore[current] + CalculateCost(current, neighbor);
                if (!openSet.Contains(neighbor)) {
                    openSet.Add(neighbor); // Discover a new cell
                    
                    neighbor.State.pathStateVisuals = "open";
                    neighbor.UpdateVisuals();
                } else if (tentativeGScore >= gScore[neighbor]) {
                    continue; // Not a better path
                }

                // This path is the best until now. Record it!
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endCell);
            }
            yield return new WaitForSeconds(pathfindingSpeed);
        }

        Debug.Log("No path found!");
        // Reset the selection for the next pathfinding
        startCell = null;
        endCell = null;

        pathfindingCoroutine = null;
    }

    // Helper method to reconstruct the path from start to goal
    private List<CellScript> ReconstructPath(Dictionary<CellScript, CellScript> cameFrom, CellScript current) {
        List<CellScript> path = new List<CellScript>();
        path.Add(current);
        
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }
        
        path.Reverse(); // Reverse to get path from start to goal
        return path;
    }

    private float CalculateCost(CellScript a, CellScript b) {
        return 1;
    }

    private float Heuristic(CellScript a, CellScript b) {
        // Using Manhattan distance as heuristic
        return Mathf.Abs(a.State.x - b.State.x) + Mathf.Abs(a.State.y - b.State.y);
    }

    private CellScript GetLowestFScoreCell(List<CellScript> openSet, Dictionary<CellScript, float> fScore) {
        CellScript lowest = openSet[0];
        foreach (var cell in openSet) {
            if (fScore[cell] < fScore[lowest]) {
                lowest = cell;
            }
        }
        return lowest;
    }

    public List<CellScript> GetNeighbors(CellScript cell, bool includeDiagonals = false) {
        List<CellScript> neighbors = new List<CellScript>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (Mathf.Abs(x) == Mathf.Abs(y) && !includeDiagonals) continue; // Skip diagonal neighbors
                CellState neighborState = GridManager.Instance.GetCellStateByIndex(cell.State.x + x, cell.State.y + y);
                if (neighborState != null) {
                    neighbors.Add(GridManager.Instance.grid[neighborState.x, neighborState.y]);
                }
            }
        }
        return neighbors;
    }

    // Advances the simulation by one step
    void SimulationStep() 
    {
        // Calculate the next state for all cells
        // Store all of the updated cells in a new array so that we don't "contaminate" the cells
        // in state "time" with the cells in state "time + 1".
        CellState[,] nextState = new CellState[gridW, gridH];
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                nextState[x,y] = grid[x,y].GenerateNextSimulationStep();
            }
        }

        // Apply the new states (now that we are done updating all the cells)
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                grid[x,y].State = nextState[x,y];
                grid[x,y].UpdateVisuals();
            }
        }
    }

    // Gets a cell state with wrapping at grid boundaries
    public CellState GetCellStateByIndexWithWrap(int x, int y) {
        // Wrap coordinates to stay within grid bounds
        x = (x + gridW) % gridW;
        y = (y + gridH) % gridH;
        return grid[x,y].State;
    }

    // Returns null if it is out of bounds
    public CellState GetCellStateByIndex(int x, int y) {
        if (x < gridW && x >= 0 && y < gridH && y >= 0) {
            return grid[x,y].State;
        }
        return null;
    }

    // Converts a world position to grid indices
    Vector2Int WorldPointToGridIndices(Vector3 worldPoint) {
        Vector2Int gridPosition = new Vector2Int();
        gridPosition.x = Mathf.FloorToInt(worldPoint.x / (cellWidth + spacing));
        gridPosition.y = Mathf.FloorToInt(worldPoint.z / (cellHeight + spacing));
        return gridPosition;
    }

    // Creates the grid of cells
    public void GenerateGrid() {
        // Clear any existing cells
        for (int i = transform.childCount-1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        // Initialize the grid array
        grid = new CellScript[gridW, gridH];
        
        // Create each cell in the grid
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                // Calculate position based on cell size and spacing
                Vector3 pos = new Vector3((cellWidth+spacing) * x, 0, (cellHeight+spacing) * y);
                
                // Instantiate the cell and get its script component
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
                CellScript cs = cell.GetComponent<CellScript>();
                
                // Initialize cell state with Perlin noise for height variation
                cs.State.height = Mathf.PerlinNoise(Time.time + x/5f, Time.time + y/5f) * maxHeight;
                cs.State.x = x;
                cs.State.y = y;
                
                // Set cell size and parent
                cell.transform.localScale = new Vector3(cellWidth, 1, cellHeight);
                cell.transform.SetParent(transform);
                
                // Store reference in the grid array
                grid[x,y] = cs;
            }
        }
    }
}

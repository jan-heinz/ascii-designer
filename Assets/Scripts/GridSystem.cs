using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;
    public Vector3 gridOrigin = Vector3.zero;

    [Header("Visual Settings")]
    public GameObject gridCellPrefab;
    public Material occupiedMaterial; // visual helper to shows cells that have furniture placed

    private bool[,] occupiedCells;
    private GameObject[,] gridCoords; // list of cell coordinates
    private bool isGridVisible = false;

    void Start()
    {
        occupiedCells = new bool[gridWidth, gridHeight];
        InitializeGridCoordinates();
        HideGrid();
    }

    // create coordinates and visuals for each cell of the grid
    void InitializeGridCoordinates()
    {
        gridCoords = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPos = GridToWorldPosition(x, y);
                // create the actual GameObject
                GameObject cell = Instantiate(gridCellPrefab, cellPos, Quaternion.identity, transform);
                cell.name = $"Cell@{x}_{y}";

                // populate the array
                gridCoords[x, y] = cell;
            }
        }
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        // coordinate of the center of the cell in world space
        return gridOrigin + new Vector3(x * cellSize + (cellSize / 2), y * cellSize + (cellSize / 2), 0);
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        // coordinate of the center of the cell in grid space
        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    // can the furniture of given size be placed at the given grid position?
    public bool CanPlaceFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        // check if furniture fits within grid bounds
        if (gridPos.x < 0 || gridPos.y < 0 ||
            gridPos.x + furnitureSize.x > gridWidth ||
            gridPos.y + furnitureSize.y > gridHeight)
        {
            return false;
        }

        // check for occupied cells within the furniture area
        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                if (occupiedCells[x, y])
                    return false;
            }
        }

        return true;
    }

    // add furniture to the grid, marking cells as occupied
    public void PlaceFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        // mark the relevant cells as occupied
        // starts from bottom left, then goes right and up
        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                occupiedCells[x, y] = true;

                // visual indication of occupied cells
                gridCoords[x, y].GetComponent<Image>().material = occupiedMaterial;
            }
        }
    }
    
    public void VacateFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) continue;
                occupiedCells[x, y] = false;
                var img = gridCoords[x, y].GetComponent<Image>();
                if (img) img.material = null; // revert visuals if you tint occupied cells
            }
        }
    }


    // display the grid
    // used when user drags furniture item into the world
    public void ShowGrid()
    {
        isGridVisible = true;
        foreach (var cell in gridCoords)
            cell.SetActive(true);
    }

    // display the grid
    // specifically for editor mode
    public void ShowGridEditor()
    {
        if (gridCoords == null) InitializeGridCoordinates();
        ShowGrid();
    }

    // hide the grid
    // used after user finishes dragging furniture item into the world, also in editor mode
    public void HideGrid()
    {
        if (gridCoords == null) return;

        isGridVisible = false;
        foreach (var cell in gridCoords)
            cell.SetActive(false);
    }

    // delete the grid GameObjects from the scene
    // used in editor mode
    public void DeleteGrid()
    {
        if (gridCoords.Length > 0)
        {
            foreach (var cell in gridCoords)
            {
                if (cell != null)
                    DestroyImmediate(cell);
            }
        }
        gridCoords = null;
    }
}





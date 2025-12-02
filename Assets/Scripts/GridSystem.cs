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
        if (occupiedCells == null) return false;

        // Use actual array dimensions (robust if gridWidth/Height changed mid-play)
        int maxX = occupiedCells.GetLength(0);
        int maxY = occupiedCells.GetLength(1);

        // Bounds check against array sizes
        if (gridPos.x < 0 || gridPos.y < 0 ||
            gridPos.x + furnitureSize.x > maxX ||
            gridPos.y + furnitureSize.y > maxY)
        {
            return false;
        }

        // Footprint overlap check
        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                if (occupiedCells[x, y]) return false;
            }
        }

        return true;
    }


    // add furniture to the grid, marking cells as occupied
    public void PlaceFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        if (occupiedCells == null || gridCoords == null) return;

        int maxX = occupiedCells.GetLength(0);
        int maxY = occupiedCells.GetLength(1);

        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                if (x < 0 || y < 0 || x >= maxX || y >= maxY) continue;

                occupiedCells[x, y] = true;

                var cell = gridCoords[x, y];
                if (cell != null)
                {
                    var img = cell.GetComponent<Image>();
                    if (img != null && occupiedMaterial != null)
                        img.material = occupiedMaterial;
                }
            }
        }
    }

    
    public void VacateFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        if (occupiedCells == null || gridCoords == null) return;

        int maxX = occupiedCells.GetLength(0);
        int maxY = occupiedCells.GetLength(1);

        for (int x = gridPos.x; x < gridPos.x + furnitureSize.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + furnitureSize.y; y++)
            {
                if (x < 0 || y < 0 || x >= maxX || y >= maxY) continue;

                occupiedCells[x, y] = false;

                var cell = gridCoords[x, y];
                if (cell != null)
                {
                    var img = cell.GetComponent<Image>();
                    if (img) img.material = null;
                }
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





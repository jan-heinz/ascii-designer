using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // so you can see it in Edit Mode too
public class ItemGrid : MonoBehaviour
{
    [Header("Grid")]
    public GridLayoutGroup grid;           
    public int columns = 3;
    public Vector2 cellSize = new Vector2(128, 128);
    public Vector2 spacing = new Vector2(8, 8);

    [Header("Slots")]
    public GameObject slotPrefab;         
    public int slotCount = 6;
    public List<Sprite> sprites = new List<Sprite>(); 

    private void OnValidate()
    {
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(1, columns);
            grid.cellSize = cellSize;
            grid.spacing = spacing;
        }
        if (!Application.isPlaying) Build(); // refresh in editor
    }

    // build the grid once the scene starts
    private void Start()
    {
        if (Application.isPlaying) Build();
    }

    public void Build()
    {
        // need both grid and a prefab to proceed
        if (grid == null || slotPrefab == null) return;

        // clear existing children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(transform.GetChild(i).gameObject);
            else Destroy(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        // create desired number slots
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, transform);
            go.name = $"Slot_{i:00}";

            // assign sprite to the slot 
            var slot = go.GetComponent<ItemSlot>();
            if (slot != null)
            {
                // pick the sprite at index i if it exists
                // null = empty slow
                var s = (i < sprites.Count) ? sprites[i] : null;
                slot.SetSprite(s); // refreshes the Image
            }
        }
    }
}

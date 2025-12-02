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
    public List<FurnitureItem> items = new List<FurnitureItem>();

    private bool needsRebuild = false;

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

        // refresh in editor
        if (!Application.isPlaying) needsRebuild = true;
    }

    private void Update()
    {
        // if in editor, then reset changes and rebuild
        if (!Application.isPlaying && needsRebuild)
        {
            needsRebuild = false;
            Build();
        }
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
                // null = empty slot
                var item = (i < items.Count) ? items[i] : null;

// Set the whole item so ItemSlot knows sprite, size, *and* worldScale
                slot.SetItem(item);

                // Attributes text (guard for nulls)
                if (item != null && slot.itemAttributesText != null)
                {
                    string itemAttributesString = "";
                    if (item.itemAttributes != null)
                    {
                        foreach (var attr in item.itemAttributes)
                        {
                            itemAttributesString += "• " + attr.attribute + "\n";
                        }
                    }
                    slot.itemAttributesText.text = itemAttributesString;
                }
                else if (slot.itemAttributesText != null)
                {
                    slot.itemAttributesText.text = "";
                }
                
                slot.itemCost.text = item.itemCost.ToString();
            }
        }
    }
}
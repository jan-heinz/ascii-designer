using UnityEngine;
using System.Collections.Generic;

// go to Assets --> Create --> ScriptableObjects --> FurnitureItem to create new furniture items
[CreateAssetMenu(fileName = "FurnitureItem", menuName = "ScriptableObjects/FurnitureItem")]
public class FurnitureItem : ScriptableObject
{
    [Header("Visual")]
    public Sprite sprite; // sprite representing the furniture
    [Min(0.01f)] public float worldScale = 1f;
    public Vector2Int furnitureSize = new Vector2Int(1, 1); // size in grid cells

    [Header("Gameplay")]
    public int itemCost;
    public List<FurnitureAttribute> itemAttributes; // furniture attributes, can have multiple
    //public SoundClip placeSFX; // SFX that plays on item creation

}
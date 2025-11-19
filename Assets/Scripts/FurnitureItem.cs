using UnityEngine;
using System.Collections.Generic;

// go to Assets --> Create --> ScriptableObjects --> FurnitureItem to create new furniture items
[CreateAssetMenu(fileName = "FurnitureItem", menuName = "ScriptableObjects/FurnitureItem")]
public class FurnitureItem : ScriptableObject
{
    public Sprite sprite; // sprite representing the furniture
    public Vector2Int furnitureSize = new Vector2Int(1, 1); // size in grid cells
    //public SoundClip placeSFX; // SFX that plays on item creation
    public List<FurnitureAttribute> itemAttributes; // furniture attributes, can have multiple
}
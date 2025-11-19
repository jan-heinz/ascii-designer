using UnityEngine;

// go to Assets --> Create --> ScriptableObjects --> FurnitureAttribute to create new furniture attributes
[CreateAssetMenu(fileName = "FurnitureAttribute", menuName = "ScriptableObjects/FurnitureAttribute")]
public class FurnitureAttribute : ScriptableObject
{
    public string attribute;
}
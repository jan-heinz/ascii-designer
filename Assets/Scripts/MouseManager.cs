using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // check if we hit any world-space object
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

        if (hit.collider == null || hit.collider.GetComponent<PlacedFurnitureNoGrid>() == null)
        {
            // clicked on empty space
            PlacedFurnitureNoGrid.Deselect();
        }
    }
}

using UnityEngine;

// converts a UI screen position to 2d world space and spawns SpriteRenderer at that location
public class WorldPlacement : MonoBehaviour
{
    public static WorldPlacement Instance; // global access point set during Awake

    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    public Transform worldRoot;
    public Camera worldCamera;

    private void Awake()
    {
        Instance = this; // set global reference
        if (worldCamera == null) worldCamera = Camera.main; 
    }

    // convert a UI screen position (ie pixels) to a 2D world position on z=0
    // note: uses Unity's new Input System
    public Vector3 ScreenToWorld(Vector2 screenPos)
    {
        var cam = worldCamera != null ? worldCamera : Camera.main; // fallback 
        var p = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        p.z = 0f; // project onto a 2D plane
        return p;
    }

    // spawns a new GameObject with a SpriteRenderer at the given world position
    public GameObject PlaceSprite(Sprite sprite, string nameHint, Vector3 worldPos)
    {
        if (!sprite) return null; // nothing happens if no sprite is supplied

        // create a new GameObject to hold the sprite
        var go = new GameObject(string.IsNullOrEmpty(nameHint) ? "PlacedItem" : nameHint);
        
        // parent under worldRoot (keep hierarchy clean)
        if (worldRoot) go.transform.SetParent(worldRoot, true);
        
        // place at the exact requested position 
        go.transform.position = worldPos;

        // add and configure a SpriteRenderer
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = sortingOrder;

        return go;
    }
}
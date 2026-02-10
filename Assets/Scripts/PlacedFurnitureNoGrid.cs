using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlacedFurnitureNoGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private FurnitureItem _item;
    private Vector2Int _size;
    private Vector3 _originalWorld;

    // UI ghost
    private Canvas _rootCanvas;
    private RectTransform _ghostRT;
    private Image _ghostImg;

    private SpriteRenderer _sr;
    private LevelManager _levelManager;
    private RequirementSystem _requirementSystem;
    
    [Header("Audio")]
    public AudioClip clickSFX;   // play when you start dragging a placed item
    public AudioClip placeSFX; 

    public void Init(FurnitureItem item)
    {
        _item = item;
        _size = item != null ? item.furnitureSize : Vector2Int.one;
        
        _levelManager = FindObjectOfType<LevelManager>();
        _requirementSystem = FindObjectOfType<RequirementSystem>(); 
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_rootCanvas == null) _rootCanvas = FindObjectOfType<Canvas>(); // any screen-space canvas
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();

        _originalWorld = transform.position;

        // SFX: starting to move an already-placed item
        if (clickSFX) AudioManager.Instance.PlaySFX(clickSFX);

        // Create UI ghost sized to match placed world size on screen
        CreateGhost(_sr.sprite, eventData.position, _item != null ? _item.worldScale : 1f);

        // Hide the real sprite during drag (avoids double-vision)
        if (_sr) _sr.enabled = false;
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (_ghostRT != null)
            _ghostRT.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyGhost();

        var wp = WorldPlacement.Instance;
        if (wp == null)
        {
            if (_sr) _sr.enabled = true;
            return;
        }

        // delete object
        if (PointerOverAnyUI(eventData, true))
        {
            // update requirements
            _requirementSystem.RemoveAttributes(_item);

            // update balance
            _levelManager.ReturnItem(_item.itemCost);

            // successfully deleted
            Destroy(gameObject);
            
            return;
        }

        if (!PointerOverAnyUI(eventData, false)) // can place furniture
        {
            transform.position = wp.ScreenToWorld(eventData.position);

            // SFX: successfully placed in new spot
            if (placeSFX) AudioManager.Instance.PlaySFX(placeSFX);
        }
        else
        {
            // Revert and re-occupy original footprint
            transform.position = _originalWorld;
        }

        if (_sr) _sr.enabled = true;
    }





    private void CreateGhost(Sprite s, Vector2 startPos, float worldScale)
    {
        if (_rootCanvas == null) return;

        var go = new GameObject("DragGhost(World)", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        go.transform.SetParent(_rootCanvas.transform, false);
        go.transform.SetAsLastSibling();

        _ghostRT = go.GetComponent<RectTransform>();
        _ghostRT.pivot = new Vector2(0.5f, 0.5f);

        Camera cam = (WorldPlacement.Instance != null && WorldPlacement.Instance.worldCamera != null)
            ? WorldPlacement.Instance.worldCamera
            : Camera.main;

        Vector2 sizePx = new Vector2(64f, 64f);
        if (cam && cam.orthographic && s)
        {
            Vector2 worldSize = s.bounds.size * Mathf.Max(0.01f, worldScale);
            float pxPerWorldUnit = Screen.height / (2f * cam.orthographicSize);
            sizePx = worldSize * pxPerWorldUnit;
        }

        float canvasUnitsPerPixel = 1f;
        if (_rootCanvas.renderMode != RenderMode.WorldSpace)
            canvasUnitsPerPixel = 1f / _rootCanvas.scaleFactor;

        _ghostRT.sizeDelta = sizePx * canvasUnitsPerPixel;
        _ghostRT.position  = startPos;

        _ghostImg = go.GetComponent<Image>();
        _ghostImg.sprite = s;
        _ghostImg.preserveAspect = true;
        _ghostImg.raycastTarget = false;

        var cg = go.GetComponent<CanvasGroup>();
        cg.alpha = 0.9f;
    }

    private void DestroyGhost()
    {
        if (_ghostRT != null) Destroy(_ghostRT.gameObject);
        _ghostRT = null;
        _ghostImg = null;
    }

    // returns true if the cursor is currently over any UI elements
    // prevents dropping items onto the inventory UI
    private bool PointerOverAnyUI(PointerEventData eventData, bool checkInventoryOnly = false)
    {
        if (EventSystem.current == null) return false;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool overUI = results.Count > 0;

        if (checkInventoryOnly)
        {
            overUI = false;
            foreach (var r in results)
            {
                if (r.gameObject.tag == "Inventory")
                {
                    overUI = true;
                    break;
                }
            }
        }

        results.Clear();
        return overUI;
    }
}

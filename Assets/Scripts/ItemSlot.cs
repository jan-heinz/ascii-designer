using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// represents an inventory slot that can display the ascii sprite, border, ettc
// supports dragging the item into the world to place it

// layers
// - item image: shows the assigned itemSprite
// - border image: shown only when an item is assigned to the slot
public class ItemSlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private FurnitureItem _item; // current item data (SO); may be null

    [Header("UI")]
    public Image itemImage; // assign child ItemImage
    public Image borderImage; // assign child BorderImage
    public TextMeshProUGUI itemAttributesText; // can leave blank, will be assigned through ItemGrid
    public TextMeshProUGUI itemCost; // can leave blank, will be assigned through ItemGrid

    [Header("Sprites")]
    // both are set dynamically
    public Sprite itemSprite; // grid sets
    public Sprite borderSprite; // set on prefab

    [Header("Item Info")]
    public Vector2Int itemSize = new Vector2Int(1, 1); // size in grid cells

    [Header("Audio")]
    public AudioClip clickFurnitureSFX;
    public AudioClip placeFurnitureSFX;
    

    // runtime drag ghost
    private Canvas _rootCanvas;
    private RectTransform _dragGhostRT;
    private Image _dragGhostImg;
    private GridSystem _gridSystem;
    private LevelManager _levelManager;
    private RequirementSystem _requirementSystem;

    // if true, flip to show attributes
    // if false, flip to show image
    private bool _showAttributes;

    private void Awake()
    {
        _rootCanvas = GetComponentInParent<Canvas>(); // needed to render drag ghost
        ApplyAll(); // set up border and item visibility
        _gridSystem = FindObjectOfType<GridSystem>();
        _levelManager = FindObjectOfType<LevelManager>();
        _requirementSystem = FindObjectOfType<RequirementSystem>(); 
        _showAttributes = true;
    }
    
    public void SetItem(FurnitureItem item)
    {
        _item = item;
        itemSprite = item ? item.sprite : null;                 // keep your existing sprite path
        itemSize   = item ? item.furnitureSize : new Vector2Int(1,1);
        ApplyItemOnly();
    }
    
    private float GetWorldScaleForPlacement()
    {
        return _item ? Mathf.Max(0.01f, _item.worldScale) : 1f; // fallback if no SO
    }

#if UNITY_EDITOR
    private void OnValidate() { ApplyAll(); } // lets you see changes when editing in inspector
#endif

    // called by the grid/controller to assign an item to this slot
    public void SetSprite(Sprite s)
    {
        itemSprite = s;
        ApplyItemOnly();
    }

    // applies both the border and item state
    private void ApplyAll()
    {
        // border setup
        if (borderImage)
        {
            if (borderSprite != null) borderImage.sprite = borderSprite;
            borderImage.raycastTarget = false;   // won't consume UI clicks
            borderImage.preserveAspect = false;  // stretch to cell
        }
        ApplyItemOnly();
    }

    // refresh what is affected by item assignment which is item sprite and border visibility
    private void ApplyItemOnly()
    {
        // item layer
        if (itemImage)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = (itemSprite != null); // hide when empty
            itemImage.preserveAspect = true;
        }

        // border only if there is an item
        if (borderImage)
        {
            bool hasItem = (itemSprite != null);
            borderImage.enabled = hasItem; // empty item slot = no border shown
        }
    }

    // start click
    // flip over the slot to show the furniture attributes
    public void OnPointerClick(PointerEventData eventData)
    {
        FlipSlot();
    }

    private void FlipSlot()
    {
        // show or disable the furniture image accordingly
        itemImage.enabled = !_showAttributes;

        // show or disable the attributes text accordingly
        itemAttributesText.enabled = _showAttributes;

        _showAttributes = !_showAttributes;
    }

    // start drag
    // if slot has an item, create a ghost that follows the cursor
    public void OnBeginDrag(PointerEventData eventData)
    {
        // only allow dragging if this slot has an item
        if (itemSprite == null) return;

        // play SFX
        AudioManager.Instance.PlaySFX(clickFurnitureSFX);

        if (_rootCanvas == null) _rootCanvas = GetComponentInParent<Canvas>();
        CreateDragGhost(itemSprite, eventData.position); // spawn ghost under cursor

        // show item placement grid
        if (_gridSystem != null) _gridSystem.ShowGrid();

        // ensure slot is flipped to image side
        _showAttributes = false;
        FlipSlot();
    }

    // while dragging
    // move the ghost to the cursor position
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhostRT != null)
        {
            _dragGhostRT.position = eventData.position; // follow cursor
        }
    }

    // end drag
    // destroy the ghost and place the sprite in the world
    public void OnEndDrag(PointerEventData eventData)
{
    DestroyDragGhost(); // remove ghost
    if (_gridSystem != null) _gridSystem.HideGrid(); // hide item placement grid

    // place only if: not over UI, we have an item, we have a level manager, and we can afford it
    if (!PointerOverAnyUI(eventData) && _item != null && _levelManager != null && _levelManager.CanAfford(_item.itemCost))
    {
        var placer = WorldPlacement.Instance;
        if (placer != null && itemSprite != null && _gridSystem != null)
        {
            Vector2Int gridPos = _gridSystem.WorldToGridPosition(eventData.position);

            // validate placement
            if (_gridSystem.CanPlaceFurniture(gridPos, itemSize))
            {
                Vector3 snappedScreenPos = _gridSystem.GridToWorldPosition(gridPos.x, gridPos.y);

                Vector3 world = placer.ScreenToWorld(snappedScreenPos);

                // occupy the grid cells
                _gridSystem.PlaceFurniture(gridPos, itemSize);

                // spawn the placed object
                GameObject placedObject = placer.PlaceSprite(itemSprite, itemSprite.name, world);
                // play placement SFX
                AudioManager.Instance.PlaySFX(placeFurnitureSFX);
                
                if (placedObject) placedObject.transform.localScale = Vector3.one * GetWorldScaleForPlacement();

                // make it draggable later
                if (placedObject)
                {
                    // ensure collider for pointer hits
                    var col = placedObject.GetComponent<BoxCollider2D>();
                    if (col == null)
                    {
                        col = placedObject.AddComponent<BoxCollider2D>();
                        var sr = placedObject.GetComponent<SpriteRenderer>();
                        if (sr && sr.sprite) col.size = sr.sprite.bounds.size; // local units
                    }

                    var mover = placedObject.AddComponent<PlacedFurniture>();
                    mover.Init(_gridSystem, gridPos, _item); // pass grid, current cell, and the FurnitureItem
                }

                Debug.Log($"Placed furniture at grid position: {gridPos} and world position: {world}");

                // purchase cost
                _levelManager.PurchaseItem(_item.itemCost);

                // update requirements
                _requirementSystem.CheckItem(_item);
            }
            else
            {
                Debug.Log($"Can't place furniture at {gridPos} - Invalid position or occupied!");
            }
        }
    }
}



    // create semi transparent UI image that follows the cursor during drag
    private void CreateDragGhost(Sprite s, Vector2 startPos)
    {
        var go = new GameObject("DragGhost", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        go.transform.SetParent(_rootCanvas.transform, false);
        go.transform.SetAsLastSibling();

        _dragGhostRT = go.GetComponent<RectTransform>();
        _dragGhostRT.pivot = new Vector2(0.5f, 0.5f);

        // compute ghost size to match placed size on screen (ortho)
        // choose a camera: prefer WorldPlacement's camera, else Camera.main
        Camera cam = (WorldPlacement.Instance != null && WorldPlacement.Instance.worldCamera != null)
            ? WorldPlacement.Instance.worldCamera
            : Camera.main;

        // default/fallback size in case camera/sprite is missing or non-ortho
        Vector2 sizePx = new Vector2(64f, 64f);

        if (cam != null && cam.orthographic && s != null)
        {
            // sprite size in WORLD units at scale=1
            Vector2 worldSize = s.bounds.size * GetWorldScaleForPlacement();

            // pixels per world unit for an ortho camera
            float pxPerWorldUnit = Screen.height / (2f * cam.orthographicSize);

            // convert world size → screen pixels
            sizePx = worldSize * pxPerWorldUnit;
        }

        // convert pixels → Canvas units (accounts for CanvasScaler scaleFactor)
        float canvasUnitsPerPixel = 1f;
        if (_rootCanvas != null && _rootCanvas.renderMode != RenderMode.WorldSpace)
            canvasUnitsPerPixel = 1f / _rootCanvas.scaleFactor;

        _dragGhostRT.sizeDelta = sizePx * canvasUnitsPerPixel;  // final ghost size
        _dragGhostRT.position  = startPos;

        _dragGhostImg = go.GetComponent<Image>();   // use item icon
        _dragGhostImg.sprite = s;                   // keep item properties
        _dragGhostImg.preserveAspect = true;
        _dragGhostImg.raycastTarget = false;

        var cg = go.GetComponent<CanvasGroup>();
        cg.alpha = 0.9f; // slightly transparent
    }

    // destroys the drag ghost if it exists
    private void DestroyDragGhost()
    {
        if (_dragGhostRT != null) Destroy(_dragGhostRT.gameObject);
        _dragGhostRT = null;
        _dragGhostImg = null;
    }

    // returns true if the cursor is currently over any UI elements
    // prevents dropping items onto the inventory UI
    private bool PointerOverAnyUI(PointerEventData eventData)
    {
        if (EventSystem.current == null) return false;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool overUI = results.Count > 0;
        results.Clear();
        return overUI;
    }
}

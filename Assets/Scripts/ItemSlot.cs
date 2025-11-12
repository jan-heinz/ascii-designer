using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// represents an inventory slot that can display the ascii sprite, border, ettc
// supports dragging the item into the world to place it

// layers
// - item image: shows the assigned itemSprite
// - border image: shown only when an item is assigned to the slot
public class ItemSlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image itemImage; // assign child ItemImage
    public Image borderImage; // assign child BorderImage

    [Header("Sprites")]
    // both are set dynamically
    public Sprite itemSprite; // grid sets
    public Sprite borderSprite; // set on prefab

    // runtime drag ghost
    private Canvas _rootCanvas;
    private RectTransform _dragGhostRT;
    private Image _dragGhostImg;

    private void Awake()
    {
        _rootCanvas = GetComponentInParent<Canvas>(); // needed to render drag ghost
        ApplyAll(); // set up border and item visibility
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
    
    // start drag
    // if slot has an item, create a ghost that follows the cursor
    public void OnBeginDrag(PointerEventData eventData)
    {
        // only allow dragging if this slot has an item
        if (itemSprite == null) return;

        if (_rootCanvas == null) _rootCanvas = GetComponentInParent<Canvas>();
        CreateDragGhost(itemSprite, eventData.position); // spawn ghost under cursor
    }

    // while dragging
    // move the ghost to the cursor position
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhostRT != null)
            _dragGhostRT.position = eventData.position; // follow cursor
    }

    // end drag
    // destroy the ghost and place the sprite in the world
    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyDragGhost(); // remove ghost

        // only place if the cursor is not over any UI element (ie inventory)
        if (!PointerOverAnyUI(eventData))
        {
            var placer = WorldPlacement.Instance;
            if (placer != null && itemSprite != null)
            {
                Vector3 world = placer.ScreenToWorld(eventData.position); // convert the UI screen position to world
                placer.PlaceSprite(itemSprite, itemSprite.name, world); // spawn a world SpriteRenderer at that position
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
        _dragGhostRT.sizeDelta = new Vector2(64, 64); // ghost size
        _dragGhostRT.position = startPos;

        _dragGhostImg = go.GetComponent<Image>(); // use item icon
        _dragGhostImg.sprite = s; // keep item properties
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

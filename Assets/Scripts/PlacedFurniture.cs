using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Attach to placed furniture so it can be dragged to a new grid cell.
/// Reuses your GridSystem + WorldPlacement flow.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlacedFurniture : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GridSystem _grid;
    private FurnitureItem _item;
    private Vector2Int _size;
    private Vector2Int _currentGrid;
    private Vector3 _originalWorld;

    // UI ghost
    private Canvas _rootCanvas;
    private RectTransform _ghostRT;
    private Image _ghostImg;

    private SpriteRenderer _sr;

    public void Init(GridSystem grid, Vector2Int gridPos, FurnitureItem item)
    {
        _grid = grid;
        _item = item;
        _size = item != null ? item.furnitureSize : Vector2Int.one;

        _currentGrid = (_grid != null)
            ? _grid.WorldToGridPosition(transform.position)
            : gridPos;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_grid == null) return;

        if (_rootCanvas == null) _rootCanvas = FindObjectOfType<Canvas>();
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();

        _originalWorld = transform.position;
        _grid.ShowGrid();
        
        Camera cam = (WorldPlacement.Instance != null && WorldPlacement.Instance.worldCamera != null)
            ? WorldPlacement.Instance.worldCamera
            : Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
            _currentGrid = _grid.WorldToGridPosition(screenPos); // pass SCREEN coords here
        }

        _grid.VacateFurniture(_currentGrid, _size);

        CreateGhost(_sr.sprite, eventData.position, _item != null ? _item.worldScale : 1f);

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
        if (_grid == null || wp == null)
        {
            if (_sr) _sr.enabled = true;
            return;
        }

        Vector2Int newGrid = _grid.WorldToGridPosition(eventData.position);

        if (_grid.CanPlaceFurniture(newGrid, _size))
        {
            Vector3 snappedScreen = _grid.GridToWorldPosition(newGrid.x, newGrid.y);
            Vector3 snappedWorld  = wp.ScreenToWorld(snappedScreen);

            transform.position = snappedWorld;
            _grid.PlaceFurniture(newGrid, _size);
            _currentGrid = newGrid;
        }
        else
        {
            transform.position = _originalWorld;
            _grid.PlaceFurniture(_currentGrid, _size);
        }

        if (_sr) _sr.enabled = true;
        _grid.HideGrid();
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
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestingToTakeAwayTile : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Targets & Damage")]
    public string[] targetTags;
    public int damage = 1;

    [Header("World Interaction")]
    public LayerMask worldLayerMask = ~0;

    [Header("Usage Settings")]
    public int remainingUses = 1;
    public bool destroyWhenDepleted = true;
    public float destroyDelayAfterUse = 0.05f;

    [Header("UI References")]
    public Text remainingUsesText;

    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 dragOffset;
    private LayoutElement layoutElement;

    public string powerId;

    private Camera ActiveCamera => Camera.main;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>(true)?.rootCanvas;
        layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = false;
        UpdateRemainingUsesText();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        layoutElement.ignoreLayout = true;

        if (parentCanvas)
            rectTransform.SetParent(parentCanvas.transform, true);

        transform.SetAsLastSibling();

        
        if (remainingUsesText)
            remainingUsesText.enabled = false;

        RectTransform canvasRect = parentCanvas.transform as RectTransform;
        Camera uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, uiCamera, out Vector2 localPointerPos);
        dragOffset = (Vector2)rectTransform.localPosition - localPointerPos;

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData) => SetDraggedPosition(eventData);

    public void OnEndDrag(PointerEventData eventData)
    {
        bool wasUsed = TryUsePowerOnWorld(eventData.position);

        if (wasUsed)
        {
            remainingUses = Mathf.Max(0, remainingUses - 1);
            UpdateRemainingUsesText();

            if (remainingUses <= 0 && destroyWhenDepleted)
            {
                Destroy(gameObject, destroyDelayAfterUse);
                return;
            }
        }

       
        if (remainingUsesText && remainingUses > 0)
            remainingUsesText.enabled = true;

        if (originalParent && remainingUses > 0)
        {
            rectTransform.SetParent(originalParent, true);
            rectTransform.SetSiblingIndex(originalSiblingIndex);
        }

        layoutElement.ignoreLayout = false;
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        RectTransform canvasRect = parentCanvas.transform as RectTransform;
        Camera uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, uiCamera, out Vector2 localPointerPos))
            rectTransform.localPosition = localPointerPos + dragOffset;
    }

    private bool TryUsePowerOnWorld(Vector2 screenPosition)
    {
        Vector3 worldPoint = ActiveCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, worldLayerMask);
        if (!hit.collider) return false;

        if (!IsAllowedTarget(hit.collider.tag)) return false;

        BackgroundTile tile = hit.collider.GetComponent<BackgroundTile>();
        if (tile != null)
        {
            tile.TakeDamage(damage);
            return true;
        }

       
        Destroy(hit.collider.gameObject);
        return true;
    }

    private bool IsAllowedTarget(string tagToCheck)
    {
        if (targetTags == null || targetTags.Length == 0)
            return false;

        foreach (string tag in targetTags)
        {
            if (tagToCheck == tag)
                return true;
        }

        return false;
    }

    public void UpdateRemainingUsesText()
    {
        if (remainingUsesText)
        {
            remainingUsesText.text = remainingUses.ToString();
            remainingUsesText.enabled = remainingUses > 0;
        }
    }
}

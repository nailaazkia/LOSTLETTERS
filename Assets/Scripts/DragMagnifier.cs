using UnityEngine;
using UnityEngine.EventSystems;

public class DragMagnifier : MonoBehaviour, IDragHandler
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta;
    }
}
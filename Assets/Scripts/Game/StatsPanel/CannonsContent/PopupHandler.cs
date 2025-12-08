using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PopupHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform popupWindow;
    public Canvas canvas;
    public Vector2 popUpOffset;
    public void OnPointerEnter(PointerEventData eventData)
    {
        popupWindow.gameObject.SetActive(true);
        UpdatePopupPosition(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popupWindow.gameObject.SetActive(false);
    }

    void Update()
    {
        if (popupWindow.gameObject.activeSelf)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            UpdatePopupPosition(mousePos);
        }
    }

    void UpdatePopupPosition(Vector2 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out localPoint
        );

        Vector2 targetPos = localPoint + popUpOffset;

        RectTransform container = popupWindow.parent as RectTransform;

        Vector2 popupSize = popupWindow.rect.size;
        Vector2 containerSize = container.rect.size;

        // pivot from center point
        float halfWidth = popupSize.x * 0.5f;
        float halfHeight = popupSize.y * 0.5f;

        float minX = -containerSize.x / 2f + halfWidth;
        float maxX =  containerSize.x / 2f - halfWidth;
        float minY = -containerSize.y / 2f + halfHeight;
        float maxY =  containerSize.y / 2f - halfHeight;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        popupWindow.localPosition = targetPos;
    }


}
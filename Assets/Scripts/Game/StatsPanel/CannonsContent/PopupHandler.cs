using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PopupHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform popupWindow; //popupwindow reference
    public Canvas canvas; //space in which position is calculated
    public Vector2 popUpOffset; //offset
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
            null,
            out localPoint
        );

        Vector2 targetPos = localPoint + popUpOffset;

        RectTransform container = popupWindow.parent as RectTransform; //positon limit from parent
        Rect rect = container.rect;
        Vector2 containerSize = container.rect.size;

        Vector2 popupSize = popupWindow.rect.size;
        
        //POPUP FLIP
        // if right side of popup is over panel
        if (targetPos.x + popupSize.x > rect.xMax)
        {
            // move popup to left side of mouse
            targetPos.x = localPoint.x - popUpOffset.x - popupSize.x;
        }

        // if upper side of popup is over panel
        if (targetPos.y + popupSize.y > rect.yMax)
        {
            // move popup under mouse
            targetPos.y = localPoint.y - popUpOffset.y - popupSize.y;
        }
        //pivot from (0,0) left down side of popup
        float minX = rect.xMin;
        float maxX = rect.xMax - popupSize.x;
        float minY = rect.yMin;
        float maxY = rect.yMax - popupSize.y;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        popupWindow.localPosition = targetPos;
    }
    public void HidePopup()
    {
        popupWindow.gameObject.SetActive(false);
    }

}
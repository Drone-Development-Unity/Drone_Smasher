using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Used by ShopWindowPopupCurrencyElement to set values on instantiate
/// </summary>
public class SetCurrencyDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currAmount;
    [SerializeField] private Image currIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetCurrencyData(int amount, Sprite icon)
    {
        currAmount.text = amount.ToString();
        currIcon.sprite = icon;
    }
}

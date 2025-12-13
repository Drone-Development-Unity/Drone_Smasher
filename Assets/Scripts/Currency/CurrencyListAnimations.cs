using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyListAnimations : MonoBehaviour
{
    [SerializeField] private Image panelImage;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("CurrencyNotSufficientAnimation Settings")]
    [SerializeField] private Color notSufficientBGColor;
    [SerializeField] private Color notSufficientTextColor;
    [SerializeField] private int colorChangesLoops; //2- back and forth once
    [SerializeField] private float colorChangeDuration;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeForce;
    
    private Color originalBGColor;
    private Color originalTextColor;
    
    void Awake()
    {
        //original values
        originalBGColor = panelImage.color;
        originalTextColor = amountText.color;
    }
    public void CurrencyNotSufficientAnimation()
    {
        panelImage.DOColor(notSufficientBGColor,colorChangeDuration)
            .SetLoops(colorChangesLoops, LoopType.Yoyo)
            .OnComplete(() => panelImage.color = originalBGColor);

        amountText.DOColor(notSufficientTextColor,colorChangeDuration)
            .SetLoops(colorChangesLoops, LoopType.Yoyo)
            .OnComplete(() => amountText.color = originalTextColor);

        transform.DOShakePosition(
            duration: shakeDuration, // duration
            strength: new Vector3(shakeForce, shakeForce, 0f), // force
            vibrato: 10, // shakes amount
            randomness: 90f, // direction randomness
            snapping: false, // values snapping
            fadeOut: true // end animation fadeout
        );
    } 
    void OnDestroy()
    {
        DOTween.Kill(panelImage);
        DOTween.Kill(amountText);
        DOTween.Kill(transform);
    }
}

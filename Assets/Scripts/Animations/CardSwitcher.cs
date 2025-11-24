using UnityEngine;
using DG.Tweening;

public class CardSwitcher : MonoBehaviour
{
    public CanvasGroup fadePanel;     // Panel do fade (alpha sterowana)
    public GameObject[] cards;        // Karty
    private int activeCardIndex = 0;
    private bool isSwitching = false; // Blokada klikniêæ w trakcie animacji

    private void Start()
    {
        fadePanel.alpha = 0f;         // Upewnia siê ¿e startujemy bez fade
        cards[activeCardIndex].SetActive(true);

        // Wy³¹cz resztê kart  
        for (int i = 1; i < cards.Length; i++)
            cards[i].SetActive(false);
    }

    public void SwitchCard(int newCardIndex)
    {
        if (newCardIndex < 0 || newCardIndex >= cards.Length) return;
        if (newCardIndex == activeCardIndex) return;
        if (isSwitching) return;

        isSwitching = true;

        fadePanel.DOKill(true);

        Sequence seq = DOTween.Sequence();

        seq.Append(fadePanel.DOFade(1f, 0.15f).SetEase(Ease.Linear)) // FADE OUT
           .AppendCallback(() =>
           {
               // Dopiero tu zmieniamy kartê — gdy alpha = 1
               cards[activeCardIndex].SetActive(false);
               cards[newCardIndex].SetActive(true);
               activeCardIndex = newCardIndex;
           })
           .Append(fadePanel.DOFade(0f, 0.15f).SetEase(Ease.Linear)) // FADE IN
           .OnComplete(() =>
           {
               isSwitching = false;
           });
    }
}
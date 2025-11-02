using UnityEngine;

public class SelectGameMenu : MonoBehaviour
{
    [Header("Popup Objects")]
    public GameObject backgroundDim;
    public GameObject infoPopup;
    public GameObject confirmPopup;
    public GameObject warningPopup;

    [Header("Canvas Groups")]
    public CanvasGroup infoGroup; // przypisz InfoPopup tutaj

    public void ShowInfo()
    {
        backgroundDim.SetActive(true);
        infoPopup.SetActive(true);
        confirmPopup.SetActive(false);
        warningPopup.SetActive(false);

        infoGroup.interactable = true;
        infoGroup.blocksRaycasts = true;
    }

    public void ShowConfirm()
    {
        confirmPopup.SetActive(true);

        // info nadal widoczne, ale nieaktywne
        infoGroup.interactable = false;
        infoGroup.blocksRaycasts = false;
    }

    public void ShowWarning()
    {
        warningPopup.SetActive(true);

        infoGroup.interactable = false;
        infoGroup.blocksRaycasts = false;
    }

    public void CloseAll()
    {
        backgroundDim.SetActive(false);
        infoPopup.SetActive(false);
        confirmPopup.SetActive(false);
        warningPopup.SetActive(false);
    }

    public void BackToInfo()
    {
        confirmPopup.SetActive(false);
        warningPopup.SetActive(false);

        infoGroup.interactable = true;
        infoGroup.blocksRaycasts = true;
    }
}
using Assets.Scripts.SaveSystem.DTOs;
using Assets.Scripts.UI.Saves;
using UnityEngine;

public class SelectGameMenu : MonoBehaviour
{
    public static SelectGameMenu Instance { get; private set; }

    [Header("Popup Objects")]
    public GameObject backgroundDim;
    public GameObject infoPopup;
    public GameObject confirmPopup;
    public GameObject warningPopup;

    public SavePopUpUI infoPopUpUI;

    [Header("Canvas Groups")]
    public CanvasGroup infoGroup; // przypisz InfoPopup tutaj

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowInfo(MetaDataDTO meta)
    {
        backgroundDim.SetActive(true);
        infoPopup.SetActive(true);
        confirmPopup.SetActive(false);
        warningPopup.SetActive(false);

        infoGroup.interactable = true;
        infoGroup.blocksRaycasts = true;

        // update data
        if (infoPopUpUI != null)
            infoPopUpUI.UpdateMeta(meta);
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
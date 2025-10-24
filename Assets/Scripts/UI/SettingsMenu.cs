using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Slider audioSlider;

    private readonly Resolution[] availableResolutions = new Resolution[]
    {
        new Resolution { width = 1280, height = 720 },   // HD
        new Resolution { width = 1920, height = 1080 }   // Full HD
    };

    private void Start()
    {
        // set dropdown options
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "1280 x 720 (HD)",
            "1920 x 1080 (Full HD)"
        });
        // set toggle fullscreen state
        fullscreenToggle.isOn = Screen.fullScreen;

        if (Screen.width == 1920 && Screen.height == 1080)
            resolutionDropdown.value = 1;
        else
            resolutionDropdown.value = 0;

        resolutionDropdown.RefreshShownValue();

        // subscribe to UI events
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void ChangeResolution(int index)
    {
        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        Debug.Log($"Resolution changed: {res.width}x{res.height}");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscreen: {(isFullscreen ? "W³¹czony" : "Wy³¹czony")}");
    }
}

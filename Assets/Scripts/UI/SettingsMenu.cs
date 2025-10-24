using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Slider audioSlider;

    private Resolution[] resolutions;
    

}

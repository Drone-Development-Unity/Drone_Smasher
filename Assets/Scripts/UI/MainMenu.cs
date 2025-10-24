using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void OnQuit()
    {
    #if UNITY_EDITOR // editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else // build
        Application.Quit();
    #endif
    }
}

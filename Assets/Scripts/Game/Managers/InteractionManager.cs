using UnityEngine;

namespace Game.Managers
{
    /// <summary>
    /// Add more mouse reaction options to HighlightObject
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private HighlightObject _currentlyClickedObject;

        /// <summary>
        /// Called by HighlightObject when clicked
        /// </summary>
        public void RegisterClick(HighlightObject newClicked)
        {
            if (_currentlyClickedObject != null && _currentlyClickedObject != newClicked)
            {
                _currentlyClickedObject.OnClickExit();
            }

            //Change clicked object
            _currentlyClickedObject = newClicked;
        }
    }
}
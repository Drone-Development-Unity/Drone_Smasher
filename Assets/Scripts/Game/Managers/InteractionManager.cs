using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Managers
{
    /// <summary>
    /// Manager that search for objects to react with mouse
    /// Checks if objects implements IInteractable interface
    /// </summary>
    public class InteractionManager:MonoBehaviour
    {
        public static InteractionManager Instance;
        private IInteractable _lastHovered;
        private IInteractable _clickedObject;
        private void Awake()
        {
            Instance = this;
        }
        void Update()
        {
            if (Camera.main == null || Mouse.current == null) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            IInteractable current = hit?.GetComponent<IInteractable>();

            // Hover logic
            if (current != _lastHovered)
            {
                _lastHovered?.OnHoverExit();
                current?.OnHoverEnter();
                _lastHovered = current;
            }


            // Click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (current != null)
                {
                    if (_clickedObject != null && _clickedObject != current)
                    {
                        _clickedObject.OnClickExit(); // Uncheck previous object
                    }

                    current.OnClick(); // Check new
                    _clickedObject = current;
                }
                else
                {
                    _clickedObject?.OnClickExit(); // Uncheck when clicked on nothing
                    _clickedObject = null;
                }
            }
        }
    }
}
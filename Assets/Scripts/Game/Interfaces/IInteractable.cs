namespace Game
{
    /// <summary>
    /// Used to set basic animations for object to implement
    /// with reaction on mouse
    /// </summary>
    public interface IInteractable
    {
        void OnHoverEnter();
        void OnHoverExit();
        void OnClick();
        void OnClickExit();
    }
}


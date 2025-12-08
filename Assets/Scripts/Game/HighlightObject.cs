using DG.Tweening;
using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Determine basic mouse reactions
    /// </summary>
    public class HighlightObject : MonoBehaviour, IPointerClickHandler,IPointerExitHandler, IPointerEnterHandler
    {
        public float hoverScaleMultiplier = 1.1f;
        public float clickScaleMultiplier = 0.9f;
        public float animationDuration = 0.2f;
        public Color clickedColor = Color.gray;
        private Color _originalColor;
        private Vector3 _originalScale;
        private Tween _currentTween;
        private SpriteRenderer[] _spriteRenderers;
        protected void Start()
        {
            _originalScale = transform.localScale;
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);


            if (_spriteRenderers == null || _spriteRenderers.Length == 0)
            {
                //Debug.LogWarning("No SpriteRenderers detected!");
                return;
            }
            //First sprite determine color
            _originalColor = _spriteRenderers[0].color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOScale(_originalScale * hoverScaleMultiplier, animationDuration)
                .SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOScale(_originalScale, animationDuration)
                .SetEase(Ease.OutQuad);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            _currentTween?.Kill();

            Sequence clickSequence = DOTween.Sequence();
            
            clickSequence.Append(transform.DOScale(_originalScale * clickScaleMultiplier, 0.2f).SetEase(Ease.InOutQuad));

            // Color change 
            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = clickedColor;
            }

            _currentTween = clickSequence;
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.RegisterClick(this);
            }
        }

        public void OnClickExit()
        {
            _currentTween?.Kill();

            Sequence exitSequence = DOTween.Sequence();

            // Reset scale
            exitSequence.Append(transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad));

            // Reset color
            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = _originalColor;
            }


            _currentTween = exitSequence;
        }


        private void OnDestroy()
        {
            _currentTween?.Kill();
        }
    }
}
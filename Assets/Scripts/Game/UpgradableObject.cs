using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Implements HighlightObject and add abilities to change stats in StatsPanel
    /// </summary>
    public class UpgradableObject: HighlightObject
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            var stats = GetComponent<IStatsDisplay>().GetStats();
            if (stats != null)
            {
                GameUIManager.Instance.ShowObjectProperties(stats);
            }
        }
    }
}
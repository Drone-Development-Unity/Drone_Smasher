using Game.Managers;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Implements HighlightObject and add abilities to change stats in StatsPanel
    /// </summary>
    public class UpgradableObject: HighlightObject
    {
        public override void OnClick()
        {
            base.OnClick();
            var stats = GetComponent<IStatsDisplay>().GetStats();
            if (stats != null)
            {
                GameUIManager.Instance.ShowObjectProperties(stats);
            }
        }
    }
}
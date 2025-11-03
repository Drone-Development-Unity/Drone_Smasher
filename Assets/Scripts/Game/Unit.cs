using UnityEngine;

namespace Game
{
    /// <summary>
    /// Set object stats to edit and display in StatsPanel UI
    /// </summary>
    public class Unit: MonoBehaviour,IStatsDisplay
    {
        [SerializeField] private StatsData _stats;

        public StatsData GetStats()
        {
            return _stats;
        }
    }
}
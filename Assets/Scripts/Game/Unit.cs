using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Set object stats to edit and display in StatsPanel UI
    /// </summary>
    public class Unit: MonoBehaviour,IStatsDisplay
    {
        [SerializeField] private StatsData _stats;

        private void Awake()
        {
            foreach(var upgrade in _stats.upgrades)
            {
                upgrade.baseObject = gameObject;
            }
        }

        public StatsData GetStats()
        {
            return _stats;
        }
    }
}
using Assets.Scripts.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Wave
{
    public class NextWaveManager : MonoBehaviour
    {
        // Singleton instance
        [HideInInspector] public static NextWaveManager Instance { get; private set; }

        private int aliveEnemies = 0;
        private WaveSpawner _waveSpawner;
        private SelectWaveManager _selectWaveManager;

        // Create Singleton
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }
        private void Start()
        {
            _waveSpawner = WaveSpawner.Instance;
            _selectWaveManager = SelectWaveManager.Instance;
        }

        public void EnemyKilled(BaseEnemy enemy)
        {
            _waveSpawner.UnregisterEnemy(enemy.GetID());
            aliveEnemies--;

            if (aliveEnemies <= 0)
            {
                _selectWaveManager.ShowUI();
            }
        }

        public void RegisterEnemies(int count)
        {
            aliveEnemies += count;
        }

        public int GetAliveEnemiesCount()
        {
            return aliveEnemies;
        }
    }
}

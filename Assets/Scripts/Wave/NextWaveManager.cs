using Assets.Scripts.Enemies;
using Assets.Scripts.SaveSystem;
using PlayerBase;
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
        private int killedEnemies = 0;
        private WaveSpawner _waveSpawner;
        private SelectWaveManager _selectWaveManager;
        private WaveTimer _waveTimer;

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
            _waveTimer = WaveTimer.Instance;
        }

        public void EnemyKilled(BaseEnemy enemy)
        {
            _waveSpawner.UnregisterEnemy(enemy.GetID());
            aliveEnemies--;
            killedEnemies++;

            // END WAVE MOMENT
            if (aliveEnemies <= 0 &&
                killedEnemies >= _waveSpawner.GetEnemiesToSpawn()
                )
            {
                PlayerBaseHealth.Instance.ResetHP();
                GameSaveHandler.Instance.SaveGameState();

                _waveTimer.ResetTimer();
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

        public void OnNewWave()
        {
            killedEnemies = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Wave
{
    public class NextWaveManager : MonoBehaviour
    {
        // Singleton instance
        [HideInInspector] public static NextWaveManager Instance { get; private set; }

        private int aliveEnemies = 0;
        private WaveSpawner waveSpawner;

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
            waveSpawner = WaveSpawner.Instance;
        }

        public void EnemyKilled()
        {
            aliveEnemies--;
            if (aliveEnemies <= 0)
            {
                waveSpawner.SpawnWave();
            }
        }

        public void RegisterEnemies(int count)
        {
            aliveEnemies += count;
        }
    }
}

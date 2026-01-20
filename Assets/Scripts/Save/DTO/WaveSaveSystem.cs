using Assets.Scripts.Save.DTO.wave;
using Assets.Scripts.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Save.DTO
{
    public class WaveSaveSystem : MonoBehaviour
    {
        [HideInInspector] public static WaveSaveSystem Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public WaveDTO SaveDTO()
        {
            return new WaveDTO
            {
                waveSpawner = WaveSpawner.Instance.SaveDTO(),
                selectWave = SelectWaveManager.Instance.SaveDTO()
            };
        }
        public void LoadDTO(WaveDTO dto)
        {
            WaveSpawner.Instance.LoadDTO(dto.waveSpawner);
            SelectWaveManager.Instance.LoadDTO(dto.selectWave);
        }
    }
}

using Assets.Scripts.Save.DTO.wave;
using Assets.Scripts.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Wave
{
    public class SelectWaveManager : MonoBehaviour
    {
        private int selectedWaveNumber;
        private WaveSpawner _waveSpawner;

        // Singleton
        [HideInInspector] public static SelectWaveManager Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI waveNumberText;
        [SerializeField] private Button previousWaveBtn;
        [SerializeField] private Button nextWaveBtn;
        [SerializeField] private Button startWaveBtn;

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
            UpdateUI();
        }

        public void NextWave()
        {
            if(selectedWaveNumber < _waveSpawner.GetAvaliableWaveNumber())
            {
                selectedWaveNumber++;
                UpdateUI();
            }
        }

        public void PreviousWave()
        {
            if(selectedWaveNumber > 1)
            {
                selectedWaveNumber--;
                UpdateUI();
            }
        }

        public void StartSelectedWave()
        {
            //Debug.Log($"Starting Wave: {selectedWaveNumber}");

            // SAVE GAME
            GameSaveHandler.Instance.SaveGameState();

            _waveSpawner.SpawnWave(selectedWaveNumber);

            HideUI();
        }

        private void UpdateUI()
        {
            waveNumberText.text = $"Level: {selectedWaveNumber}";

            nextWaveBtn.interactable = selectedWaveNumber < _waveSpawner.GetAvaliableWaveNumber();
            previousWaveBtn.interactable = selectedWaveNumber > 1;
        }

        private void SetUIVisible(bool visible)
        {
            nextWaveBtn.gameObject.SetActive(visible);
            previousWaveBtn.gameObject.SetActive(visible);
            startWaveBtn.gameObject.SetActive(visible);

            UpdateUI();
        }

        public void ShowUI() => SetUIVisible(true);
        public void HideUI() => SetUIVisible(false);

        // MEMENTO
        public SelectWaveDTO SaveDTO()
        {
            return new SelectWaveDTO { selectedWaveNumber = selectedWaveNumber };
        }
        public void LoadDTO(SelectWaveDTO dto)
        {
            this.selectedWaveNumber = dto.selectedWaveNumber;
            UpdateUI();
        }
    }
}

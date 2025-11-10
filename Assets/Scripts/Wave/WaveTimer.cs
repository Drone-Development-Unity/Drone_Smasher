using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Wave
{
    public class WaveTimer : MonoBehaviour
    {
        public static WaveTimer Instance { get; private set; }
        private WaveSpawner waveSpawner;

        [Header("Timer Settings")]
        private float timeLeft;
        private bool isTimerActive;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI uiTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            waveSpawner = WaveSpawner.Instance;
        }

        private void Update()
        {
            if (!isTimerActive) return;

            timeLeft -= Time.deltaTime;

            // end of timer
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                isTimerActive = false;
            }

            UpdateUITimer(timeLeft);
        }

        public void StartTimer(int durationInSeconds)
        {
            timeLeft = durationInSeconds;
            isTimerActive = true;

            UpdateUITimer(timeLeft);
        }

        public bool IsWaveTimeOver => !isTimerActive || timeLeft <= 0f;

        private void UpdateUITimer(float timeLeft)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            uiTimer.text = $"{minutes:D2}:{seconds:D2}";
        }

    }

}

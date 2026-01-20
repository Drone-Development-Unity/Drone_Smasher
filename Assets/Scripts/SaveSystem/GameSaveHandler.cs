using Assets.Scripts.SaveSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Save.DTO;
using UnityEngine;
using Assets.Scripts.Save.DTO;

namespace Assets.Scripts.SaveSystem
{
    // SAVE MANAGER at
    // GAME SCENE
    public class GameSaveHandler : MonoBehaviour
    {
        public static GameSaveHandler Instance { get; private set; }

        // save current state
        public GameDataDTO CurrentGameSave { get; private set; }
        public MetaDataDTO CurrentMetaSave { get; private set; }
        public int CurrentSlot { get; private set; } = -1;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            LoadLastGameState();
        }

        // trigger this if you want 
        // LOAD GAME STATE
        public void LoadLastGameState() 
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogError("ERROR: SaveManager does not exist in the scene!");
                return;
            }

            int? slot = SaveManager.Instance.SelectedSlot;
            if (!slot.HasValue)
            {
                Debug.LogError("ERROR: No save slot selected before scene change!");
                return;
            }

            CurrentSlot = slot.Value;
            CurrentGameSave = SaveManager.Instance.LoadGame(CurrentSlot);
            CurrentMetaSave = SaveManager.Instance.GetMetaData(CurrentSlot);

            if (CurrentGameSave == null || CurrentMetaSave == null)
            {
                Debug.LogError("ERROR: Save was selected but could not be loaded!");
                return;
            }

            if (!CurrentMetaSave.Exists)
            {
                Debug.Log("INFO: Selected save slot is empty, nothing to load yet.");
            }

            //  --- DEBUG -----

            Debug.Log($"Loaded Save Slot: {CurrentSlot}");

            string metaJson = JsonUtility.ToJson(CurrentMetaSave, true);
            //Debug.Log(metaJson);

            string gameJson = JsonUtility.ToJson(CurrentGameSave, true);
            //Debug.Log(gameJson);

            //  --------------

            SetGameState(CurrentGameSave);
        }

        // trigger this if you want 
        // SAVE GAME STATE
        [ContextMenu("Zapisz grę")]
        public void SaveGameState()
        {
            // tutaj pobierz sb wszystkie DTO z managerów i wywołaj
            GameDataDTO newSave = new();
            
            //DTO
            List<MainBaseDataDTO> mainBaseDTO = new();
            var currencyDTO = new CurrenciesDTO();
            GatherersDataDTO gatherersDTO = new();
            //cannons
            var saveManager =  GameObject.FindGameObjectWithTag("SaveDataManager");
            var MainBaseSave = saveManager.GetComponent<MainBaseStateSave>();
            if (MainBaseSave) mainBaseDTO = MainBaseSave.SaveToDTO();
            
            //currencies
            currencyDTO = CurrencyManager.Instance.ConvertToDTO();
            
            //gatherers
            var gatherersSave = saveManager.GetComponent<DroneSaveSystem>();
            if (gatherersSave) gatherersDTO = gatherersSave.SaveGatherers();
            //Debug.Log($"Saved gatherers {gatherersDTO.amount}");
            // wave
            var waveSave = WaveSaveSystem.Instance.SaveDTO();
            //Save DTO to GameDataDTO
            newSave.mainBaseDataDTO = mainBaseDTO;
            newSave.currenciesDTO = currencyDTO;
            newSave.gatherersDTO = gatherersDTO;
            newSave.waveDTO = waveSave;



            SaveManager.Instance.SaveGame(CurrentSlot, newSave);
            //Debug.Log($"Saved Save Slot: {CurrentSlot}");
        }

        // Passes gameSaveDto to managers
        private void SetGameState(GameDataDTO gameSave)
        {
            //currencies
            if (gameSave.currenciesDTO == null || gameSave.currenciesDTO?.currencies.Count == 0)
            {
                // FIRST LOAD
                CurrencyManager.Instance.CreateAndLoadCurrencies();
                SaveManager.Instance.OnFirstSave(CurrentSlot);
            }
            else
            {
                CurrencyManager.Instance.LoadCurrencyFromDTO(gameSave.currenciesDTO);
            }

            //cannons
            var saveManager =  GameObject.FindGameObjectWithTag("SaveDataManager");
            var MainBaseSave = saveManager.GetComponent<MainBaseStateSave>();
            if (MainBaseSave) MainBaseSave.LoadFromDTO(gameSave.mainBaseDataDTO);
            
            //gatherers
            var gatherersSave = saveManager.GetComponent<DroneSaveSystem>();
            if(gatherersSave)gatherersSave.LoadGatherers(gameSave.gatherersDTO);

            // wave
            WaveSaveSystem.Instance.LoadDTO(gameSave.waveDTO);
            
            Debug.Log($"Game Loaded Save Slot: {CurrentSlot}");
        }
    }
}
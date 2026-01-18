using Assets.Scripts.SaveSystem.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.SaveSystem
{
    // SAVE MANAGER at
    // MAIN MENU SCENE
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const int SAVE_SLOTS = 3;
        private string SaveDir =>
            Path.Combine(Application.persistentDataPath, "saves");

        private int? selectedSlot = null;
        public void SetSelectedSlot(int slot)
        {
            this.selectedSlot = slot;
        }
        public int? SelectedSlot => selectedSlot;

        private void Awake()
        {
            Debug.Log("SaveManager start");

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSaves();
        }

        // ================= INIT =================
        private void InitializeSaves()
        {
            Directory.CreateDirectory(SaveDir);

            for (int i = 1; i <= SAVE_SLOTS; i++)
            {
                string path = GetSlotPath(i);

                if (!File.Exists(path))
                {
                    CreateEmptySave(i);
                }
            }
        }

        private void CreateEmptySave(int slot)
        {
            SaveFileDTO emptySave = new SaveFileDTO
            {
                Meta = new MetaDataDTO
                {
                    SlotId = slot,
                    Exists = false,
                    Level = 0,
                    PlayedHours = 0f,
                    CreatedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    LastPlayed = "-"
                },
                Game = new GameDataDTO()
            };

            WriteSaveFile(slot, emptySave);
        }

        // ================= PATH =================
        private string GetSlotPath(int slot)
            => Path.Combine(SaveDir, $"save_slot_{slot}.json");

        // ================= FILE IO =================
        private void WriteSaveFile(int slot, SaveFileDTO save)
        {
            string json = JsonUtility.ToJson(save, true);

            Debug.Log("Save nr:" + slot);
            Debug.Log(json);
            File.WriteAllText(GetSlotPath(slot), json);
        }

        private SaveFileDTO ReadSaveFile(int slot)
        {
            string path = GetSlotPath(slot);
            if (!File.Exists(path)) return null;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveFileDTO>(json);
        }

        // ================= META =================
        public MetaDataDTO GetMetaData(int slot)
        {
            var save = ReadSaveFile(slot);
            return save?.Meta;
        }
        public MetaDataDTO[] GetAllMetaData()
        {
            MetaDataDTO[] metas = new MetaDataDTO[SAVE_SLOTS];

            for (int i = 1; i <= SAVE_SLOTS; i++)
            {
                metas[i - 1] = GetMetaData(i);
            }

            return metas;
        }

        // ================= SAVE GAME =================
        public void SaveGame(int slot, GameDataDTO gameDto)
        {
            if(gameDto == null)
            {
                Debug.LogWarning("Could not save game: gameDto is null!");
                return;
            }

            SaveFileDTO save = ReadSaveFile(slot);

            if(save == null)
            {
                Debug.LogWarning($"Could not save game: Save slot {slot} does not exist!");
                return;
            }

            save.Game = gameDto;
            save.Meta.Exists = true;

            //save.Meta.Level = gameDto.currentWave;
            save.Meta.Level = 0;

            save.Meta.LastPlayed = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            WriteSaveFile(slot, save);
        }

        // ================= LOAD GAME =================
        public GameDataDTO LoadGame(int slot)
        {
            SaveFileDTO save = ReadSaveFile(slot);

            if (save == null)
            {
                Debug.LogWarning("Error while loading save: Save file does not exist!");
                return null;
            }

            // return Save or blank save
            return save.Game ?? new GameDataDTO();
        }

        // ================= CLEAR =================
        public void ClearSave(int slot)
        {
            CreateEmptySave(slot);
        }
    }
}

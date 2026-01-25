using Assets.Scripts.SaveSystem;
using Assets.Scripts.SaveSystem.DTOs;
using Assets.Scripts.UI.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SavesMenuUI : MonoBehaviour
    {
        [SerializeField] private SaveTile[] saveTiles;

        private void Start()
        {
            InitTiles();
        }

        public void InitTiles()
        {
            MetaDataDTO[] metas = SaveManager.Instance.GetAllMetaData();

            for (int i = 0; i < saveTiles.Length; i++)
            {
                saveTiles[i].Setup(i, metas[i]);
            }
        }
    }
}

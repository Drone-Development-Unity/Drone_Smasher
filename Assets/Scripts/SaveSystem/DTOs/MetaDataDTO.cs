using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SaveSystem.DTOs
{
    [Serializable]
    public class MetaDataDTO
    {
        public int SlotId;
        public bool Exists;
        public int Level;
        public float PlayedHours;
        public string CreatedAt;
        public string LastPlayed;
    }
}

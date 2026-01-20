using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Save.DTO.wave
{
    [System.Serializable]
    public class WaveSpawnerDTO
    {
        public int avaliableWaveNumber;
        public WaveSpawnerDTO()
        {
            avaliableWaveNumber = 1;
        }
    }
}

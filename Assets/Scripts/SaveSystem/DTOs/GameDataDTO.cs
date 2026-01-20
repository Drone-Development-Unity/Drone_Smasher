using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Save.DTO.wave;
using Save.DTO;

namespace Assets.Scripts.SaveSystem.DTOs
{
    [Serializable]
    public class GameDataDTO
    {
        public List<MainBaseDataDTO> mainBaseDataDTO;
        public CurrenciesDTO currenciesDTO;
        public GatherersDataDTO gatherersDTO;
        public WaveDTO waveDTO;
    }
}

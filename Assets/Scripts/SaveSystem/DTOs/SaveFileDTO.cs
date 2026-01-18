using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SaveSystem.DTOs
{
    [Serializable]
    public class SaveFileDTO
    {
        public MetaDataDTO Meta;
        public GameDataDTO Game;
    }
}

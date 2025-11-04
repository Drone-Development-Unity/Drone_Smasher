using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Used by PropertyObject prefab
    /// </summary>
    public class PropertyUI:MonoBehaviour
    {
        public TextMeshProUGUI propertyName;
        public TextMeshProUGUI propertyValue;
        
        public void SetProperty(PropertyData data)
        {
            propertyName.text = data.propertyName;
            propertyValue.text = data.propertyValue.ToString();
        }
    }
}
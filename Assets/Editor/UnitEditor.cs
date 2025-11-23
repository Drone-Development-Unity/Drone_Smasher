using Game;
using Game.StatsPanel;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace Editor
{
    [CustomEditor(typeof(Unit))]
    public class UnitEditor : UnityEditor.Editor
    {
        private StatsData stats;
        private Unit unit;
        private ReorderableList propertyList;
        private ReorderableList upgradeList;

        private void OnEnable()
        {
            unit = (Unit)target;
            stats = GetStatsData(unit);

            if (stats != null)
            {
                propertyList = new ReorderableList(stats.properties, typeof(PropertyData), true, true, true, true);
                propertyList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties");
                propertyList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = stats.properties[index];
                    float half = rect.width / 2;
                    prop.propertyType = (StatType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, half - 5, EditorGUIUtility.singleLineHeight), prop.propertyType);
                    prop.propertyValue = EditorGUI.DoubleField(new Rect(rect.x + half + 5, rect.y, half - 5, EditorGUIUtility.singleLineHeight), prop.propertyValue);
                };

                upgradeList = new ReorderableList(stats.upgrades, typeof(UpgradeData), true, true, true, true);
                upgradeList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Upgrades");
                upgradeList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var upgrade = stats.upgrades[index];
                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float padding = 2f;
                    float fullWidth = rect.width;
                    float labelWidth = 100f;
                    float fieldWidth = fullWidth - labelWidth - 10f;

                    Rect imageRect = new Rect(rect.x, rect.y, fullWidth, lineHeight);
                    Rect nameRect = new Rect(rect.x, rect.y + lineHeight + padding, fullWidth, lineHeight);
                    Rect costRect = new Rect(rect.x, rect.y + 2 * (lineHeight + padding), fullWidth, lineHeight);
                    Rect amountRect = new Rect(rect.x, rect.y + 3 * (lineHeight + padding), fullWidth, lineHeight);
                    Rect targetRect = new Rect(rect.x, rect.y + 4 * (lineHeight + padding), fullWidth, lineHeight);
                    Rect currencyIdRect = new Rect(rect.x, rect.y + 5 * (lineHeight + padding), fullWidth, lineHeight);

                    upgrade.upgradeImage = (Sprite)EditorGUI.ObjectField(imageRect, "Upgrade Image", upgrade.upgradeImage, typeof(Sprite), false);
                    upgrade.upgradeName = EditorGUI.TextField(nameRect, "Upgrade Name", upgrade.upgradeName);
                    upgrade.upgradeCost = EditorGUI.DoubleField(costRect, "Upgrade Cost", upgrade.upgradeCost);
                    upgrade.upgradeAmount = EditorGUI.DoubleField(amountRect, "Upgrade Amount", upgrade.upgradeAmount);
                    upgrade.currencyId = EditorGUI.IntField(currencyIdRect, "Currency ID", upgrade.currencyId);

                    string[] propertyNames = stats.properties.ConvertAll(p => p.propertyType.ToString()).ToArray();
                    int selected = Mathf.Max(0, System.Array.IndexOf(propertyNames, upgrade.targetPropertyName));
                    selected = EditorGUI.Popup(targetRect, "Target Property", selected, propertyNames);
                    upgrade.targetPropertyName = propertyNames.Length > 0 ? propertyNames[selected] : "";
                    
                    //Draw line after each UI element
                    Rect separatorRect = new Rect(rect.x, rect.y + 6 * (lineHeight + padding)+2f, fullWidth, 1f);
                    EditorGUI.DrawRect(separatorRect, new Color(0.7f, 0.7f, 0.7f)); 
                };

                upgradeList.elementHeightCallback = index =>
                {
                    // 6 fields
                    return 6 * (EditorGUIUtility.singleLineHeight + 2f)+5f;//5f - spacing
                };

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            unit = (Unit)target;
            stats = GetStatsData(unit);

            if (stats == null)
            {
                EditorGUILayout.HelpBox("Brak danych StatsData.", MessageType.Warning);
                return;
            }
            
            stats.objectName = EditorGUILayout.TextField("Object Name", stats.objectName);
            stats.description = EditorGUILayout.TextField("Description", stats.description);

            EditorGUILayout.Space();
            propertyList?.DoLayoutList();
            
            EditorGUILayout.Space();
            upgradeList?.DoLayoutList();            

            if (GUI.changed)
            {
                EditorUtility.SetDirty(unit);
            }

            serializedObject.ApplyModifiedProperties();
        }


        private StatsData GetStatsData(Unit unit)
        {
            var field = typeof(Unit).GetField("_stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(unit) as StatsData;
        }
    }
}

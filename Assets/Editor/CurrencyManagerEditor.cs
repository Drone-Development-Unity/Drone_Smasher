using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTools
{
    [CustomEditor(typeof(CurrencyManager))]
    public class CurrencyManagerEditor : UnityEditor.Editor
    {
        private CurrencyManager manager;
        private ReorderableList currencyList;

        private void OnEnable()
        {
            manager = (CurrencyManager)target;

            if (manager != null)
            {
                currencyList = new ReorderableList(manager.currencies, typeof(CurrencyData), true, true, true, true);

                currencyList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Currencies"); };

                currencyList.onAddCallback = list =>
                {
                    manager.currencies.Add(new CurrencyData
                    {
                        currencyId = 0,
                        currencyName = "New Currency",
                        icon = null,
                        amount = 0
                    });
                };
                currencyList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var currency = manager.currencies[index];
                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float padding = 2f;
                    float idWidth = 40f;
                    Rect idLabelRect = new Rect(rect.x, rect.y, idWidth, lineHeight);
                    Rect idFieldRect = new Rect(rect.x, rect.y + lineHeight, idWidth, lineHeight);

                    Rect nameLabelRect = new Rect(rect.x + idWidth + 10, rect.y, rect.width - idWidth - 10, lineHeight);
                    Rect nameFieldRect = new Rect(rect.x + idWidth + 10, rect.y + lineHeight, rect.width - idWidth - 10,
                        lineHeight);

                    EditorGUI.LabelField(idLabelRect, "ID");
                    currency.currencyId = EditorGUI.IntField(idFieldRect, currency.currencyId);

                    EditorGUI.LabelField(nameLabelRect, "Name");
                    currency.currencyName = EditorGUI.TextField(nameFieldRect, currency.currencyName);
                    float iconWidth = 60f;
                    float secondLineY = rect.y + 2 * lineHeight + padding;

                    Rect iconLabelRect = new Rect(rect.x, secondLineY, iconWidth, lineHeight);
                    Rect iconFieldRect = new Rect(rect.x, secondLineY + lineHeight, iconWidth, lineHeight);

                    Rect amountLabelRect = new Rect(rect.x + iconWidth + 10, secondLineY, rect.width - iconWidth - 10,
                        lineHeight);
                    Rect amountFieldRect = new Rect(rect.x + iconWidth + 10, secondLineY + lineHeight,
                        rect.width - iconWidth - 10, lineHeight);

                    EditorGUI.LabelField(iconLabelRect, "Icon");
                    currency.icon = (Sprite)EditorGUI.ObjectField(iconFieldRect, currency.icon, typeof(Sprite), false);

                    EditorGUI.LabelField(amountLabelRect, "Amount");
                    currency.amount = EditorGUI.DoubleField(amountFieldRect, currency.amount);
                };
                currencyList.elementHeightCallback = index => { return 4 * EditorGUIUtility.singleLineHeight + 2f; };
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //manual add fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currencyListElement"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("necessariesPanelObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currenciesContainer"));
            
            if (currencyList != null)
            {
                currencyList.DoLayoutList();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(manager);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
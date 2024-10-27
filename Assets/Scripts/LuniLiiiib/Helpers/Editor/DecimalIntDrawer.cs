using LuniLib.Helpers;

#if UNITY_EDITOR
namespace IshLib.Helpers.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(DecimalInt))]
    public class DecimalIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            SerializedProperty precisionProperty = property.FindPropertyRelative("<Precision>k__BackingField");
            float valueFloat = valueProperty.intValue;
            float precision = precisionProperty.intValue;
            valueFloat /= precision;
            valueFloat = EditorGUI.FloatField(position, label, valueFloat);
            valueProperty.intValue = Mathf.RoundToInt(valueFloat * precision);
        }
    }
}
#endif
using UnityEditor;
using UnityEngine;

namespace LuniLib.UnityUtils
{
#if UNITY_EDITOR
#endif

	[System.Serializable]
	public sealed class SceneField
	{
		#pragma warning disable CS0414
        [SerializeField] private Object sceneAsset = null;
		#pragma warning restore CS0414
        [SerializeField] private string sceneName = string.Empty;

        public string SceneName => this.sceneName;

        public static implicit operator string(SceneField sceneField)
		{
			return sceneField.SceneName;
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SceneField))]
	public sealed class SceneFieldPropertyDrawer : PropertyDrawer
	{
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			
			SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
			SerializedProperty sceneName = property.FindPropertyRelative("sceneName");
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			if (sceneAsset != null)
			{
				sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if (sceneAsset.objectReferenceValue != null)
					sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
			}

			EditorGUI.EndProperty();
		}
	}
#endif
}
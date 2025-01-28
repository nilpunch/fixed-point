using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
{
	[CustomPropertyDrawer(typeof(FP))]
	public class FPropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProperty = property.FindPropertyRelative(nameof(FP.RawValue));

			EditorGUI.BeginProperty(position, label, valueProperty);
			{
				var propertyValue = FP.FromRaw(valueProperty.longValue).ToFloat();

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.FloatField(position, new GUIContent(property.displayName), propertyValue);
				if (EditorGUI.EndChangeCheck())
				{
					valueProperty.longValue = newValue.ToFP().RawValue;
				}
			}
			EditorGUI.EndProperty();
		}
	}
}

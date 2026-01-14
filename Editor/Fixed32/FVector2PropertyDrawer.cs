using UnityEditor;
using UnityEngine;

namespace Fixed32.Editor
{
	[CustomPropertyDrawer(typeof(FVector2))]
	public class FVector2PropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var xProperty = property.FindPropertyRelative(nameof(FVector2.X)).FindPropertyRelative(nameof(FP.RawValue));
			var yProperty = property.FindPropertyRelative(nameof(FVector2.Y)).FindPropertyRelative(nameof(FP.RawValue));

			EditorGUI.BeginProperty(position, label, property);
			{
				var xValue = FP.FromRaw(xProperty.intValue).ToFloat();
				var yValue = FP.FromRaw(yProperty.intValue).ToFloat();

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector2Field(position, new GUIContent(property.displayName), new Vector2(xValue, yValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.intValue = newValue.x.ToFP().RawValue;
					yProperty.intValue = newValue.y.ToFP().RawValue;
				}

				EditorGUIUtility.wideMode = wideMode;
			}
			EditorGUI.EndProperty();
		}
	}
}

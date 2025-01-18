using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
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
				var xValue = (float)FP.FromRaw(xProperty.longValue);
				var yValue = (float)FP.FromRaw(yProperty.longValue);

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector2Field(position, new GUIContent(property.displayName), new Vector2(xValue, yValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.longValue = ((FP)newValue.x).RawValue;
					yProperty.longValue = ((FP)newValue.y).RawValue;
				}
			}
			EditorGUI.EndProperty();
		}
	}
}

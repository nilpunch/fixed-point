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
				var xValue = FP.FromRaw(xProperty.longValue).ToFloat();
				var yValue = FP.FromRaw(yProperty.longValue).ToFloat();

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector2Field(position, new GUIContent(property.displayName), new Vector2(xValue, yValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.longValue = newValue.x.ToFP().RawValue;
					yProperty.longValue = newValue.y.ToFP().RawValue;
				}

				EditorGUIUtility.wideMode = wideMode;
			}
			EditorGUI.EndProperty();
		}
	}
}

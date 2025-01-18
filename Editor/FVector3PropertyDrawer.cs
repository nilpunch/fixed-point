using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
{
	[CustomPropertyDrawer(typeof(FVector3))]
	public class FVector3PropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var xProperty = property.FindPropertyRelative(nameof(FVector3.X)).FindPropertyRelative(nameof(FP.RawValue));
			var yProperty = property.FindPropertyRelative(nameof(FVector3.Y)).FindPropertyRelative(nameof(FP.RawValue));
			var zProperty = property.FindPropertyRelative(nameof(FVector3.Z)).FindPropertyRelative(nameof(FP.RawValue));

			EditorGUI.BeginProperty(position, label, property);
			{
				var xValue = (float)FP.FromRaw(xProperty.longValue);
				var yValue = (float)FP.FromRaw(yProperty.longValue);
				var zValue = (float)FP.FromRaw(zProperty.longValue);

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector3Field(position, new GUIContent(property.displayName), new Vector3(xValue, yValue, zValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.longValue = ((FP)newValue.x).RawValue;
					yProperty.longValue = ((FP)newValue.y).RawValue;
					zProperty.longValue = ((FP)newValue.z).RawValue;
				}
				
				EditorGUIUtility.wideMode = wideMode;
			}
			EditorGUI.EndProperty();
		}
	}
}

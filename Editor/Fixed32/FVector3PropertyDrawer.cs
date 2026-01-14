using UnityEditor;
using UnityEngine;

namespace Fixed32.Editor
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
				var xValue = FP.FromRaw(xProperty.intValue).ToFloat();
				var yValue = FP.FromRaw(yProperty.intValue).ToFloat();
				var zValue = FP.FromRaw(zProperty.intValue).ToFloat();

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector3Field(position, new GUIContent(property.displayName), new Vector3(xValue, yValue, zValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.intValue = newValue.x.ToFP().RawValue;
					yProperty.intValue = newValue.y.ToFP().RawValue;
					zProperty.intValue = newValue.z.ToFP().RawValue;
				}

				EditorGUIUtility.wideMode = wideMode;
			}
			EditorGUI.EndProperty();
		}
	}
}

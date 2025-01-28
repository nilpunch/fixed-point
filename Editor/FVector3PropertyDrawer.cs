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
				var xValue = FP.FromRaw(xProperty.longValue).ToFloat();
				var yValue = FP.FromRaw(yProperty.longValue).ToFloat();
				var zValue = FP.FromRaw(zProperty.longValue).ToFloat();

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = true;

				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.Vector3Field(position, new GUIContent(property.displayName), new Vector3(xValue, yValue, zValue));
				if (EditorGUI.EndChangeCheck())
				{
					xProperty.longValue = newValue.x.ToFP().RawValue;
					yProperty.longValue = newValue.y.ToFP().RawValue;
					zProperty.longValue = newValue.z.ToFP().RawValue;
				}
				
				EditorGUIUtility.wideMode = wideMode;
			}
			EditorGUI.EndProperty();
		}
	}
}

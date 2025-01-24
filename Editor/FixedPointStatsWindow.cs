using System;
using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
{
	public class FixedPointStatsWindow : EditorWindow
	{
		[SerializeField] private float _testAngle = 30;

		private SerializedObject _serializedObject;
		
		[MenuItem("Window/Fixed Point/Stats")]
		public static void ShowWindow()
		{
			FixedPointStatsWindow wnd = GetWindow<FixedPointStatsWindow>();
			wnd.titleContent = new GUIContent("FP Stats");
		}

		private void OnEnable()
		{
			_serializedObject = new SerializedObject(this);
		}

		private void OnGUI()
		{
			const float log10Of2 = 0.30103f;
			int decimalDigitsOfAccuracy = Mathf.CeilToInt(log10Of2 * FP.FractionalPlaces);

			_serializedObject.Update();

			EditorGUILayout.Space(5f);

			EditorGUILayout.TextField("Fractional Places", FP.FractionalPlaces.ToString());
			EditorGUILayout.TextField("Integer Places", FP.IntegerPlaces.ToString());
			EditorGUILayout.Space(10f);

			EditorGUILayout.TextField("Max Integer Value", ((long)FMath.Floor(FP.MaxValue)).ToString());
			EditorGUILayout.TextField("Min Integer Value", ((long)FMath.Floor(FP.MinValue)).ToString());
			EditorGUILayout.TextField("Max Integer To Sqr", ((long)FMath.Sqrt(FP.MaxValue)).ToString());
			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Absolute Epsilon ", FP.Epsilon.ToString("G5"));
			EditorGUILayout.TextField("Calculations Epsilon ", FP.CalculationsEpsilon.ToString("G5"));
			EditorGUILayout.TextField("Calculations Epsilon Sqr ", FP.CalculationsEpsilonSqr.ToString("G5"));

			EditorGUILayout.Space(10f);
			EditorGUILayout.TextField("Pi", FP.Pi.ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("One Degrees In Rad", FP.Deg2Rad.ToString("F" + decimalDigitsOfAccuracy));

			FMath.Init();

			EditorGUILayout.Space(10f);
			EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_testAngle)), new GUIContent("Angle"));
			_serializedObject.ApplyModifiedProperties();
			EditorGUILayout.TextField("Sin", FMath.Sin(FP.Deg2Rad * (FP)_testAngle).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Sin", Math.Sin(0.017453292519943295 * _testAngle).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Sin(FP.Deg2Rad * (FP)_testAngle) - Math.Sin(0.017453292519943295 * _testAngle)).ToString("G5"));
			// EditorGUILayout.TextField("Sin Accurate", FMath.SinAcc(FP.Deg2Rad * (FP)_testAngle).ToString("F" + decimalDigitsOfAccuracy));
			// EditorGUILayout.TextField("Delta Accurate", Math.Abs((double)FMath.SinAcc(FP.Deg2Rad * (FP)_testAngle) - Math.Sin(0.017453292519943295 * _testAngle)).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Sin (MaxValue)", ((double)FMath.Sin(FP.MaxValue)).ToString("G5"));
			EditorGUILayout.TextField("Actual Sin (MaxValue)", Math.Sin((double)FP.MaxValue).ToString("G5"));
			EditorGUILayout.TextField("Delta (MaxValue)", Math.Abs((double)FMath.Sin(FP.MaxValue) - Math.Sin((double)FP.MaxValue)).ToString("G5"));
		}
	}
}

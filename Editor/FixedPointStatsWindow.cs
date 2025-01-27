using System;
using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
{
	public class FixedPointStatsWindow : EditorWindow
	{
		[SerializeField] private float _testAngle = 30;
		[SerializeField, Range(-1, 1)] private float _testValue = 0;

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

			EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_testAngle)), new GUIContent("Test Angle"));
			_serializedObject.ApplyModifiedProperties();
			var testFp = FP.Deg2Rad * (FP)_testAngle;
			var testRadians = 0.017453292519943295 * _testAngle;

			EditorGUILayout.TextField("Sin", FMath.Sin(testFp).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Sin", Math.Sin(testRadians).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Sin(testFp) - Math.Sin(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			EditorGUILayout.TextField("Cos", ((double)FMath.Cos(testFp)).ToString("G5"));
			EditorGUILayout.TextField("Actual Cos", Math.Cos(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Cos(testFp) - Math.Cos(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			EditorGUILayout.TextField("Tan", ((double)FMath.Tan(testFp)).ToString("G5"));
			EditorGUILayout.TextField("Actual Tan", Math.Tan(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Tan(testFp) - Math.Tan(testRadians)).ToString("G5"));

			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Sin (MaxValue)", ((double)FMath.Sin(FP.MaxValue)).ToString("G5"));
			EditorGUILayout.TextField("Actual Sin (MaxValue)", Math.Sin((double)FP.MaxValue).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Sin(FP.MaxValue) - Math.Sin((double)FP.MaxValue)).ToString("G5"));
			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Cos (MaxValue)", ((double)FMath.Cos(FP.MaxValue)).ToString("G5"));
			EditorGUILayout.TextField("Actual Cos (MaxValue)", Math.Cos((double)FP.MaxValue).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Cos(FP.MaxValue) - Math.Cos((double)FP.MaxValue)).ToString("G5"));
			
			EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_testValue)), new GUIContent("Test Value"));
			_serializedObject.ApplyModifiedProperties();
			var testValueFp = (FP)_testValue;

			EditorGUILayout.TextField("Arcsin", FMath.Asin(testValueFp).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Arcsin", Math.Asin(_testValue).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Asin(testValueFp) - Math.Asin(_testValue)).ToString("G5"));
		}
	}
}

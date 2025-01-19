using System;
using UnityEditor;
using UnityEngine;

namespace Mathematics.Fixed.Editor
{
	public class FixedPointStatsWindow : EditorWindow
	{
		private float _testAngle = 30;
		
		[MenuItem("Window/Fixed Point/Stats")]
		public static void ShowWindow()
		{
			FixedPointStatsWindow wnd = GetWindow<FixedPointStatsWindow>();
			wnd.titleContent = new GUIContent("FP Stats");
		}

		private void OnGUI()
		{
			const float log10Of2 = 0.30103f;
			int decimalDigitsOfAccuracy = Mathf.CeilToInt(log10Of2 * FP.FractionalPlaces);

			EditorGUILayout.Space(5f);

			EditorGUIUtility.editingTextField = false;

			EditorGUILayout.TextField("Fractional Places", FP.FractionalPlaces.ToString());
			EditorGUILayout.TextField("Integer Places", FP.IntegerPlaces.ToString());
			EditorGUILayout.Space(10f);

			EditorGUILayout.TextField("Max Integer Value", ((long)FMath.Floor(FP.MaxValue)).ToString());
			EditorGUILayout.TextField("Min Integer Value", ((long)FMath.Floor(FP.MinValue)).ToString());
			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Absolute Epsilon ", FP.Epsilon.ToString("G5"));
			EditorGUILayout.TextField("Calculations Epsilon ", FP.CalculationsEpsilon.ToString("G5"));
			EditorGUILayout.TextField("Calculations Epsilon Sqr ", FP.CalculationsEpsilonSqr.ToString("G5"));

			EditorGUILayout.Space(10f);
			EditorGUILayout.TextField("Pi", FP.Pi.ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("One Degrees In Rad", FP.Deg2Rad.ToString("F" + decimalDigitsOfAccuracy));

			FMath.Init();

			EditorGUILayout.Space(10f);
			_testAngle = EditorGUILayout.FloatField("Angle", _testAngle);
			EditorGUILayout.TextField("Sin", FMath.Sin(FP.Deg2Rad * (FP)_testAngle).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Sin", Math.Sin(0.017453292519943295 * _testAngle).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs((double)FMath.Sin(FP.Deg2Rad * (FP)_testAngle) - Math.Sin(0.017453292519943295 * _testAngle)).ToString("G5"));
		}
	}
}

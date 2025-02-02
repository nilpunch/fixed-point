﻿using System;
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
			int decimalDigitsOfAccuracy = Mathf.CeilToInt(log10Of2 * FP.FractionalBits);

			_serializedObject.Update();

			EditorGUILayout.Space(5f);

			EditorGUILayout.TextField("Fractional Places", FP.FractionalBits.ToString());
			EditorGUILayout.TextField("Integer Places", FP.IntegerBits.ToString());
			EditorGUILayout.Space(10f);

			EditorGUILayout.TextField("Max Integer Value", (FMath.Floor(FP.MaxValue).ToLong()).ToString());
			EditorGUILayout.TextField("Min Integer Value", (FMath.Floor(FP.MinValue).ToLong()).ToString());
			EditorGUILayout.TextField("Max Integer To Sqr", (FMath.Sqrt(FP.MaxValue).ToLong()).ToString());
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
			var testFp = FP.Deg2Rad * _testAngle.ToFP();
			var testRadians = 0.017453292519943295 * _testAngle;

			EditorGUILayout.TextField("Sin", FMath.Sin(testFp).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Sin", Math.Sin(testRadians).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Sin(testFp).ToDouble() - Math.Sin(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			EditorGUILayout.TextField("Cos", (FMath.Cos(testFp).ToDouble()).ToString("G5"));
			EditorGUILayout.TextField("Actual Cos", Math.Cos(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Cos(testFp).ToDouble() - Math.Cos(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			EditorGUILayout.TextField("Tan", FMath.Tan(testFp).ToDouble().ToString("G5"));
			EditorGUILayout.TextField("Actual Tan", Math.Tan(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Tan(testFp).ToDouble() - Math.Tan(testRadians)).ToString("G5"));

			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Sin (MaxValue)", FMath.Sin(FP.MaxValue).ToDouble().ToString("G5"));
			EditorGUILayout.TextField("Actual Sin (MaxValue)", Math.Sin(FP.MaxValue.ToDouble()).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Sin(FP.MaxValue).ToDouble() - Math.Sin(FP.MaxValue.ToDouble())).ToString("G5"));
			EditorGUILayout.Space(5f);
			EditorGUILayout.TextField("Cos (MaxValue)", FMath.Cos(FP.MaxValue).ToDouble().ToString("G5"));
			EditorGUILayout.TextField("Actual Cos (MaxValue)", Math.Cos(FP.MaxValue.ToDouble()).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Cos(FP.MaxValue).ToDouble() - Math.Cos(FP.MaxValue.ToDouble())).ToString("G5"));
			
			EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_testValue)), new GUIContent("Test Value"));
			_serializedObject.ApplyModifiedProperties();
			var testValueFp = _testValue.ToFP();

			EditorGUILayout.TextField("Arcsin", FMath.Asin(testValueFp).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Arcsin", Math.Asin(_testValue).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.Asin(testValueFp).ToDouble() - Math.Asin(_testValue)).ToString("G5"));
			
			EditorGUILayout.TextField("Arctan", FMath.AtanSeries(testValueFp).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Arctan", Math.Atan(_testValue).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(FMath.AtanSeries(testValueFp).ToDouble() - Math.Atan(_testValue)).ToString("G5"));
		}
	}
}

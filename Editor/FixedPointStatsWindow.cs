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
		private Vector2 _scroll;
		
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
			using var scrollscope = new EditorGUILayout.ScrollViewScope(_scroll, false, true);
			_scroll = scrollscope.scrollPosition;

			if (GUILayout.Button("Log CORDIC constants"))
			{
				string sum = string.Empty;
				for (int i = 0; i < 64; ++i)
				{
					var result = Math.Atan(1.0 / Math.Pow(2, i));
					sum += (long)(result * (1UL << 63)) + ",\n";
				}
				Debug.Log(sum);

				double cos = 1f;
				for (int i = 0; i < 64; ++i)
				{
					var result = Math.Atan(1.0 / Math.Pow(2, i));
					cos *= Math.Cos(result);
				}
				Debug.Log((long)(cos * (1UL << 63)));
			}
			
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

			EditorGUILayout.Space(10f);

			EditorGUILayout.PropertyField(_serializedObject.FindProperty(nameof(_testAngle)), new GUIContent("Test Angle"));
			_serializedObject.ApplyModifiedProperties();
			var testFp = FP.Deg2Rad * _testAngle.ToFP();
			var testRadians = 0.017453292519943295 * _testAngle;

			FCordic.SinCos(testFp.RawValue, out var sinRaw, out var cosRaw);
			var sinFP = FP.FromRaw(sinRaw);
			var cosFP = FP.FromRaw(cosRaw);
			
			EditorGUILayout.TextField("Sin", sinFP.ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Sin", Math.Sin(testRadians).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(sinFP.ToDouble() - Math.Sin(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			EditorGUILayout.TextField("Cos", (cosFP.ToDouble()).ToString("G5"));
			EditorGUILayout.TextField("Actual Cos", Math.Cos(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(cosFP.ToDouble() - Math.Cos(testRadians)).ToString("G5"));
			EditorGUILayout.Space(2f);
			
			var tanFP = FP.FromRaw(FCordic.Tan(testFp.RawValue));
			EditorGUILayout.TextField("Tan", tanFP.ToDouble().ToString("G5"));
			EditorGUILayout.TextField("Actual Tan", Math.Tan(testRadians).ToString("G5"));
			EditorGUILayout.TextField("Delta", Math.Abs(tanFP.ToDouble() - Math.Tan(testRadians)).ToString("G5"));
			
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

			var asinFP = FMath.Asin(testValueFp);
			EditorGUILayout.TextField("Arcsin", asinFP.ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Arcsin", Math.Asin(_testValue).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(asinFP.ToDouble() - Math.Asin(_testValue)).ToString("G5"));
			var atanFP = FMath.Atan(testValueFp);
			EditorGUILayout.TextField("Arctan", atanFP.ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Actual Arctan", Math.Atan(_testValue).ToString("F" + decimalDigitsOfAccuracy));
			EditorGUILayout.TextField("Delta", Math.Abs(atanFP.ToDouble() - Math.Atan(_testValue)).ToString("G5"));
		}
	}
}

using System;
using UnityEngine;

namespace NeatBullets.Core.Scripts.CustomEditor.MinMaxRangeAttribute
{
	/// <summary>
	/// An attribute that simplifies defining bounded ranges (ranges with minimum and maximum limits) on the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MinMaxSliderAttribute : PropertyAttribute
	{
		public float MinValue { get; private set; }
		public float MaxValue { get; private set; }

		public MinMaxSliderAttribute(float minValue, float maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}

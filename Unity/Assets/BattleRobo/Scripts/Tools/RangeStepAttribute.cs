using System;
using UnityEngine;

[AttributeUsage (AttributeTargets.Field)]
public sealed class RangeStepAttribute : PropertyAttribute
{
	public readonly int min;
	public readonly int max;
	public readonly int step;
 
	public RangeStepAttribute (int min, int max, int step)
	{
		this.min = min;
		this.max = max;
		this.step = step;
	}
}

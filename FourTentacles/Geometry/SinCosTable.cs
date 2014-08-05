using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;

namespace FourTentacles
{
	class SinCosTable
	{
		private readonly float[] sin;
		private readonly float[] cos;
		public SinCosTable(int sides)
		{
			sin = new float[sides];
			cos = new float[sides];
			for (int i = 0; i < sides; i++)
			{
				var a = Math.PI * 2f * i / sides;
				sin[i] = (float)Math.Sin(a);
				cos[i] = (float)Math.Cos(a);
			}
		}

		public IEnumerable<Vector3> Points(Vector3 north, Vector3 west)
		{
			for (int i = 0; i < sin.Length; i++)
				yield return west * sin[i] + north * cos[i];
		}

		public int Sides { get { return sin.Length; } }
	}
}
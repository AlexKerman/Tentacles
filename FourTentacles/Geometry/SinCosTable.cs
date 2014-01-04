using System;
using OpenTK;

namespace FourTentacles
{
	class SinCosTable
	{
		private float[] sin;
		private float[] cos;
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

		public int Sides { get { return sin.Length; } }

		public Vector3 RingPoint(Vector3 top, Vector3 left, int index)
		{
			if (index >= sin.Length) index = index%sin.Length;
			return left * sin[index] + top * cos[index];
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Spline4d
	{
		class SinCosTable
		{
			private double[] sin;
			private double[] cos;
			public SinCosTable(int sides)
			{
				sin = new double[sides];
				cos = new double[sides];
				for (int i = 0; i < sides; i++)
				{
					var a = Math.PI * 2f * i / sides;
					sin[i] = Math.Sin(a);
					cos[i] = Math.Cos(a);
				}
			}

			public int Sides { get { return sin.Length; } }
			public float Sin(int side)
			{
				return (float)sin[side];
			}
			public float Cos(int side)
			{
				return (float) cos[side];
			}
		}

		class Segment4d
		{
			private Vector4 cpbp;
			private Vector4 cpep;
			private Vector4 bp;
			private Vector4 ep;

			private Vector3[,] posarray;
			private Vector3[,] normalarray;
			private Vector4 a, b, c, d;
			private int sides;

			public Segment4d(Vector4 start, Vector4 end, Vector4 startGuide, Vector4 endGuide, int sidesRadius)
			{
				this.bp = start;
				this.ep = end;
				this.cpbp = startGuide;
				this.cpep = endGuide;
				CalculateGeometry(new SinCosTable(sidesRadius));
			}

			private void Calculate()
			{
				a = 2 * (bp - ep) + 3 * (cpbp - cpep);		//relative guides version
				b = 3 * (cpep + ep - 2 * cpbp - bp);
				c = 3 * cpbp;
				d = bp;
			}

			private void CalculateGeometry(SinCosTable table)
			{
				Calculate();
				sides = table.Sides;
				posarray = new Vector3[sides + 1, sides];
				normalarray = new Vector3[sides + 1, sides];

				for (int ti = 0; ti <= sides; ti++)
				{
					float t = ti / (float)sides;
					Vector4 pos = GetPoint(t);
					Vector4 dir = GetDiff(t);       //4d direction vector

					//3d position from 4d vector
					Vector3 pos3 = new Vector3(pos.X, pos.Y, pos.Z);

					//3d direction vector
					Vector3 dir3 = new Vector3(dir.X, dir.Y, dir.Z);

					//коэффициенты для расчета нормалей
					//фактически, это и есть нормаль, если сегмент направлен вдоль оси X
					//Получается, что мы меняем местами направление сегмента и направление утолщения (W)
					//и получаем нужную нормаль
					Vector2 normfactor = new Vector2(dir3.Length, -dir.W);
					normfactor.Normalize();

					dir3.Normalize();
					Vector3 left = Vector3.Cross(Vector3.UnitY, dir3);
					left.Normalize();
					Vector3 top = Vector3.Cross(dir3, left);
					top.Normalize();

					for (int i = 0; i < sides; i++)
					{
						Vector3 ringv = left * table.Sin(i) + top * table.Cos(i);
						posarray[ti, i] = pos3 + ringv * pos.W;
						normalarray[ti, i] = ringv * normfactor.X + dir3 * normfactor.Y;
					}
				}
			}

			private Vector4 GetPoint(float t)
			{
				return (((a * t) + b) * t + c) * t + d;
			}

			private Vector4 GetDiff(float t)
			{
				return (a * 3 * t + b * 2) * t + c;
			}

			public void Render()
			{
				int stride = BlittableValueType.StrideOf(posarray);
				int[] indicies = new int[sides * 2 + 2];
				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.VertexArray);
				GL.Enable(EnableCap.NormalArray);
				GL.VertexPointer(3, VertexPointerType.Float, stride, posarray);
				GL.NormalPointer(NormalPointerType.Float, stride, normalarray);
				for (int i = 0; i < sides; i++)
				{
					for (int ii = 0; ii < sides; ii++)
					{
						indicies[ii * 2] = ii + i * sides;
						indicies[ii * 2 + 1] = ii + sides * (i + 1);
					}
					indicies[sides * 2] = i * sides;
					indicies[sides * 2 + 1] = (i + 1) * sides;
					GL.DrawElements(BeginMode.TriangleStrip, indicies.Length, DrawElementsType.UnsignedInt, indicies);
				}
				GL.Disable(EnableCap.Lighting);
				GL.Disable(EnableCap.VertexArray);
				GL.Disable(EnableCap.NormalArray);
			}
		}

		private int sidesRadius;
		private List<Segment4d> segments;

		public Spline4d(int sidesRadius)
		{
			this.sidesRadius = sidesRadius;
		}

		public void AddSegment(Vector4 start, Vector4 end, Vector4 startGuide, Vector4 endGuide)
		{
			segments.Add(new Segment4d(start, end, startGuide, endGuide, sidesRadius));
		}
	}
}

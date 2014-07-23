using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	class Segment4D : Node
	{
		private Vector4 cpbp;
		private Vector4 cpep;
		private Node4DPoint bp;
		private Node4DPoint ep;
		private Mesh mesh;

		public Segment4D(Node4DPoint start, Node4DPoint end, Vector4 startGuide, Vector4 endGuide)
		{
			this.bp = start;
			this.ep = end;
			this.cpbp = startGuide;
			this.cpep = endGuide;
		}

		private Vector4 a, b, c, d;

		private void CalculateConstants()
		{
			//relative control points version
			a = 2 * (bp.Point - ep.Point) + 3 * (cpbp - cpep);
			b = 3 * (cpep + ep.Point - 2 * cpbp - bp.Point);
			c = 3 * cpbp;
			d = bp.Point;
		}

		public void Render(RenderMode renderMode)
		{
			mesh.Render(renderMode);
		}

		public override void DrawContour(Camera camera, Vector3 basePos)
		{
			mesh.Render(RenderMode.Solid);
		}

		public int GetTrianglesCount()
		{
			return mesh.GetTrianglesCount();
		}

		private void CalculateIndicies(int sides, int length)
		{
			for (int ti = 0; ti < length - 1; ti++)
			{
				int offset = ti * sides;
				var indicies = new int[sides * 2 + 2];
				for (int i = 0; i < sides; i++)
				{
					indicies[i * 2] = i + offset;
					indicies[i * 2 + 1] = i + sides + offset;
				}
				indicies[sides * 2] = offset;
				indicies[sides * 2 + 1] = offset + sides;
				mesh.AddTriangleStripIndicies(indicies);

				var lineIndicies = new int[sides + 1];
				for (int i = 0; i < sides; i++)
					lineIndicies[i] = i + offset;
				lineIndicies[sides] = offset;
				mesh.AddLineStripIndicies(lineIndicies);
			}
		}

		private float[] DivideSpline(int lengthSides)
		{
			float[] t = new float[lengthSides + 1];
			for (int i = 0; i <= lengthSides; i++)
				t[i] = (float)i / (float)lengthSides;
			return t;
		}

		public void CalculateGeometry(SinCosTable table, int lengthSides)
		{
			CalculateConstants();
			int sides = table.Sides;
			var tPoints = DivideSpline(lengthSides);
			mesh = new Mesh(sides * tPoints.Length);
			var kompass = new Kompass(GetDirection(0), GetDirection(1));

			foreach (float t in tPoints)
			{
				Vector4 position = GetPoint(t);
				Vector4 direction = GetDirection(t);       //4d direction vector

				//3d direction vector
				Vector3 dir3 = direction.Xyz;
				
				//коэффициенты для расчета нормалей
				//фактически, это и есть нормаль, если сегмент направлен вдоль оси X
				//Получается, что мы меняем местами направление сегмента и направление утолщения (W)
				//и получаем нужную нормаль
				var normfactor = new Vector2(dir3.Length, -direction.W);
				normfactor.Normalize();

				Vector3 top = kompass.GetTop(t);

				dir3.Normalize();
				Vector3 left = Vector3.Cross(top, dir3);
				left.Normalize();
				if (left.X < 0) left = -left;
				top = Vector3.Cross(dir3, left);
				top.Normalize();

				for (int i = 0; i < sides; i++)
				{
					Vector3 ringPoint = table.RingPoint(top, left, i);
					mesh.AddPoint(position.Xyz + ringPoint * position.W, ringPoint * normfactor.X + dir3 * normfactor.Y);
				}
			}
			CalculateIndicies(sides, tPoints.Length);
		}

		private Vector4 GetPoint(float t)
		{
			return (((a * t) + b) * t + c) * t + d;
		}

		private Vector4 GetDirection(float t)
		{
			return (a * 3 * t + b * 2) * t + c;
		}

		public BoundingBox GetBoundingBox()
		{
			return mesh.GetBoundingBox();
		}
	}

	class Kompass
	{
		private static Vector3[] axes = {Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ};

		private Vector3 _start;
		private Vector3 _end;

		public Kompass(Vector4 start, Vector4 end)
		{
			_start = GetTopVector(start.Xyz);
			_end = GetTopVector(end.Xyz);
		}

		private Vector3 GetTopVector(Vector3 dir)
		{
			Vector3 result = Vector3.Zero;
			float minDot = 1.0f;
			dir.Normalize();
			foreach (var axis in axes)
			{
				var dot = Math.Abs(Vector3.Dot(dir, axis));
				if (dot < minDot)
				{
					minDot = dot;
					result = axis;
				}
			}
			return result;
		}

		public Vector3 GetTop(float t)
		{
			Vector3 result = _start * t + _end * (1.0f - t);
			result.Normalize();
			return result;
		}
	}
}

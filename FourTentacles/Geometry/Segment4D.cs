using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	class Segment4D
	{
		private Vector4 cpbp;
		private Vector4 cpep;
		private Vector4 bp;
		private Vector4 ep;
		private Mesh mesh;

		public Segment4D(Vector4 start, Vector4 end, Vector4 startGuide, Vector4 endGuide, int roundSides, int lenghtSides)
		{
			this.bp = start;
			this.ep = end;
			this.cpbp = startGuide;
			this.cpep = endGuide;
			CalculateGeometry(new SinCosTable(roundSides), lenghtSides);
		}

		private Vector4 a, b, c, d;

		public Mesh Mesh
		{
			get { return mesh; }
		}

		private void CalculateConstants()
		{
			//relative control points version
			a = 2 * (bp - ep) + 3 * (cpbp - cpep);
			b = 3 * (cpep + ep - 2 * cpbp - bp);
			c = 3 * cpbp;
			d = bp;
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

		private void CalculateGeometry(SinCosTable table, int lengthSides)
		{
			CalculateConstants();
			int sides = table.Sides;
			var tPoints = DivideSpline(lengthSides);
			mesh = new Mesh(sides * tPoints.Length);

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

				dir3.Normalize();
				Vector3 left = Vector3.Cross(Vector3.UnitY, dir3);
				left.Normalize();
				if (left.X < 0) left = -left;
				Vector3 top = Vector3.Cross(dir3, left);
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
}

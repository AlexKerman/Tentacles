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
		private Point4DController bp;
		private Point4DController ep;
		private readonly Guide4DController cpbp;
		private readonly Guide4DController cpep;
		private Mesh mesh = new SmoothLengthMesh();

		public Segment4D(Point4DController start, Point4DController end, Guide4DController startGuide, Guide4DController endGuide)
		{
			bp = start;
			ep = end;
			cpbp = startGuide;
			cpep = endGuide;
		}

		private Vector4 a, b, c, d;

		private void CalculateConstants()
		{
			//relative control points version
			a = 2 * (bp.Point - ep.Point) + 3 * (cpbp.Point - cpep.Point);
			b = 3 * (cpep.Point + ep.Point - 2 * cpbp.Point - bp.Point);
			c = 3 * cpbp.Point;
			d = bp.Point;
		}

		public override void Render(RenderContext context)
		{
			mesh.Render(context.Mode);
		}

		public int GetTrianglesCount()
		{
			return mesh.GetTrianglesCount();
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
			var tPoints = DivideSpline(lengthSides);
			var kompass = new Kompass(GetDirection(0), GetDirection(1));

            var points = new Vector3[table.Sides * tPoints.Length];
            var normals = new Vector3[table.Sides * tPoints.Length];

		    int pos = 0;
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
                kompass.CalcVectors(t, dir3);

				for (int i = 0; i < table.Sides; i++)
				{
					Vector3 ringPoint = table.RingPoint(kompass.North, kompass.West, i);
				    points[pos] = position.Xyz + ringPoint*position.W;
				    normals[pos] = ringPoint*normfactor.X + dir3*normfactor.Y;
                    pos++;
				}
			}
            mesh.Init(points, normals, tPoints.Length, table.Sides);
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

	    public Vector3 North { get; private set; }
        public Vector3 West { get; private set; }

		public void CalcVectors(float t, Vector3 dir3)
		{
			Vector3 north = _start * t + _end * (1.0f - t);
			north.Normalize();

            West = Vector3.Cross(north, dir3);
            West.Normalize();
            North = Vector3.Cross(dir3, West);
            North.Normalize();
		}
	}
}

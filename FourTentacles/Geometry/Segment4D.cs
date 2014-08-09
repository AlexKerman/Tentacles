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
		private Point4D bp;
		private Point4D ep;
		private readonly Guide4D cpbp;
		private readonly Guide4D cpep;

		public Mesh Mesh = new SmoothMesh();

		public Segment4D(Point4D start, Point4D end, Guide4D startGuide, Guide4D endGuide)
		{
			bp = start;
			ep = end;
			cpbp = startGuide;
			cpep = endGuide;

			start.Guides.Add(startGuide);
			startGuide.BasePoint = start;
			end.Guides.Add(endGuide);
			endGuide.BasePoint = end;
		}

		public bool Changed
		{
			get { return bp.Changed || ep.Changed; }
			set { bp.Changed = ep.Changed = value; }
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
			Mesh.Render(context.Mode);
		}

		public int GetTrianglesCount()
		{
			return Mesh.GetTrianglesCount();
		}

		private float[] DivideSpline(int lengthSides)
		{
			var t = new float[lengthSides + 1];
			for (int i = 0; i <= lengthSides; i++)
				t[i] = i / (float)lengthSides;
			return t;
		}

		public void CalculateGeometry(SinCosTable table, int lengthSides)
		{
			CalculateConstants();
			var tPoints = DivideSpline(lengthSides);
			var kompass = new Kompass(Vector3.Normalize(GetDirection(0).Xyz), Vector3.Normalize(GetDirection(1).Xyz));

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
                var rose = kompass.CalcWindrose(t, dir3);

				foreach (Vector3 ringPoint in table.Points(rose.North, rose.West))
				{
				    points[pos] = position.Xyz + ringPoint*position.W;
				    normals[pos] = ringPoint*normfactor.X + rose.Dir*normfactor.Y;
                    pos++;
				}
			}
            Mesh.Init(points, normals, tPoints.Length, table.Sides);

			bp.SetRose(kompass.StartRose, table);
			ep.SetRose(kompass.EndRose, table);
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
			return Mesh.GetBoundingBox();
		}
	}

	class Kompass
	{
		private static readonly Vector3[] axes = {Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ};

		public readonly Windrose StartRose;
		public readonly Windrose EndRose;

		public Kompass(Vector3 start, Vector3 end)
		{
			StartRose = new Windrose(start, GetTopVector(start));
			EndRose = new Windrose(end, GetTopVector(end));
		}

		private Vector3 GetTopVector(Vector3 dir)
		{
			Vector3 result = Vector3.Zero;
			float minDot = 1.0f;
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

		public Windrose CalcWindrose(float t, Vector3 dir3)
		{
			var north = StartRose.North * t + EndRose.North * (1.0f - t);
			north.Normalize();
            var west = Vector3.Cross(north, dir3);
            west.Normalize();
            north = Vector3.Cross(dir3, west);
            north.Normalize();
			return new Windrose(north, west, dir3);
		}
	}

	class Windrose
	{
		public Windrose(Vector3 north, Vector3 west, Vector3 dir)
		{
			North = north;
			West = west;
			Dir = dir;
		}

		public Windrose(Vector3 dir, Vector3 top)
		{
			North = top;
			Dir = dir;
			West = Vector3.Cross(North, Dir);
			West.Normalize();
		}

		public Vector3 North;
		public Vector3 West;
		public Vector3 Dir;
	}
}

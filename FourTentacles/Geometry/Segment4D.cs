using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Segment4D : Node
	{
		public Point4D bp;
		public Point4D ep;
		public Guide4D cpbp;
		public Guide4D cpep;

		private Mesh mesh = new SmoothMesh();

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
			mesh.Render(context.Mode);
		}

		public int GetTrianglesCount()
		{
			return mesh.GetTrianglesCount();
		}

		private float[] DivideSpline(int lengthSides)
		{
			var t = new float[lengthSides + 1];
			for (int i = 0; i <= lengthSides; i++)
				t[i] = i / (float)lengthSides;
			return t;
		}

		private float[] tPoints;
		private Kompass kompass;

		public void CalculateGeometry(Mesh smoothAlgorithm, SinCosTable table, int lengthSides)
		{
			mesh = smoothAlgorithm;
			CalculateConstants();
			tPoints = DivideSpline(lengthSides);
			kompass = new Kompass(cpbp.WindRose, cpep.WindRose);

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
            mesh.Init(points, normals, tPoints.Length, table.Sides);
		}

		public void DrawSpline()
		{
			Material.SetLineMaterial(Color.White);
			GL.Disable(EnableCap.DepthTest);
			GL.Begin(PrimitiveType.LineStrip);
			foreach (var t in tPoints)
				GL.Vertex3(GetPoint(t).Xyz);
			GL.End();
		}

		public Vector4 GetPoint(float t)
		{
			return (((a * t) + b) * t + c) * t + d;
		}

		public Vector4 GetDirection(float t)
		{
			return (a * 3 * t + b * 2) * t + c;
		}

		public Windrose GetWindrose(float t)
		{
			return kompass.CalcWindrose(t, GetDirection(t).Xyz);
		}

		public BoundingBox GetBoundingBox()
		{
			return mesh.GetBoundingBox();
		}
	}

	class Kompass
	{
		private readonly Windrose startRose;
		private readonly Windrose endRose;
		private readonly Quaternion rosesDirAngle;
		private readonly Quaternion rosesNorthAngle;

		public Kompass(Windrose start, Windrose end)
		{
			startRose = start;
			endRose = end;
			rosesDirAngle = Angle(start.Dir, end.Dir);
			var rotatedNorth = Vector3.Transform(start.North, rosesDirAngle);
			rosesNorthAngle = Angle(rotatedNorth, end.North);
		}

		//http://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
		private Quaternion Angle(Vector3 a, Vector3 b)
		{
			var cross = Vector3.Cross(a, b);
			var w = (float) (Math.Sqrt(a.LengthSquared * b.LengthSquared) + Vector3.Dot(a, b));
			return new Quaternion(cross, w).Normalized();
		}

		public Windrose CalcWindrose(float t, Vector3 dir3)
		{
			dir3.Normalize();
			var q = Angle(startRose.Dir, dir3);
			q = rosesNorthAngle.Inverted() * t;
			return new Windrose(Vector3.Transform(startRose.North, q), Vector3.Transform(startRose.West, q), dir3);
		}
	}

	struct Windrose
	{
		//All vectors must be normalized

		private static readonly Vector3[] Axes = { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };

		public Vector3 North;
		public Vector3 West;
		public Vector3 Dir;

		public Windrose(Vector3 north, Vector3 west, Vector3 dir)
		{
			North = north;
			West = west;
			Dir = dir;
		}

		public Windrose(Vector3 dir)
		{
			Dir = dir;
			North = GetTopVector(Dir);
			West = Vector3.Cross(North, Dir);
			West.Normalize();
			North = Vector3.Cross(Dir, West);
			North.Normalize();
		}

		public void Adjust(Vector3 dir)
		{
			Dir = dir;
			West = Vector3.Cross(North, Dir);
			West.Normalize();
			North = Vector3.Cross(Dir, West);
			North.Normalize();
		}

		private static Vector3 GetTopVector(Vector3 dir)
		{
			Vector3 result = Vector3.Zero;
			float minDot = 1.0f;
			foreach (var axis in Axes)
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
	}
}

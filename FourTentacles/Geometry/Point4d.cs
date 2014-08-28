using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	enum PointSmoothMode
	{
		Cusp,
		Smooth,
		Symmetrical
	}

	class Point4D : Node
	{
		private const int PointSizePx = 4;

		private Vector4 point;

		public readonly List<Guide4D> Guides = new List<Guide4D>();
		public bool Changed;

		public Vector4 Point
		{
			get { return point; }
			set
			{
				if(point == value) return;
				Changed = true;
				point = value;
			}
		}

		public PointSmoothMode SmoothMode;

		public Point4D(Vector4 point)
		{
			Point = point;
		}

		public override Vector3 Pos
		{
			get { return Point.Xyz; }
			set { Point = new Vector4(value, Point.W); }
		}

		public override void Render(RenderContext context)
		{
			if (context.Mode == RenderMode.Selection)
			{
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(Vector3.Zero);
				GL.End();
			}
			else
			{
				DrawOrthoPoint(context, Pos);
			}
		}

		public void DrawOrthoPoint(RenderContext context, Vector3 vector)
		{
			Vector2 pos = context.WorldToScreen(vector);
			int x = (int)(pos.X - PointSizePx / 2.0f);
			int y = (int)(pos.Y - PointSizePx / 2.0f);

			Material.SetLineMaterial(IsSelected ? Color.Red : Color.White);

			GL.PushMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.MatrixMode(MatrixMode.Modelview);

			context.Camera.SetOrtho();
			GL.Disable(EnableCap.LineSmooth);

			GL.Begin(PrimitiveType.LineStrip);
			GL.Vertex2(x, y);
			GL.Vertex2(x + PointSizePx, y);
			GL.Vertex2(x + PointSizePx, y + PointSizePx);
			//bug workaround (+1)
			GL.Vertex2(x, y + PointSizePx + 1);
			GL.Vertex2(x, y);
			GL.End();

			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
		}
	}
}

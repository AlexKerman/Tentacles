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
	class Point4DController : Controller
	{
		private const int PointSizePx = 4;

		private WidthController Width = new WidthController();

		public Vector4 Point
		{
			get { return new Vector4(Pos, Width.Width); }
			set 
			{
				Width.Pos = value.Xyz;
				Width.Width = value.W;
			}
		}

		public readonly List<Guide4DController> Guides = new List<Guide4DController>();

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

		protected void DrawOrthoPoint(RenderContext context, Vector3 vector)
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
			GL.Vertex2(x, y + PointSizePx);
			GL.Vertex2(x, y - 1);
			GL.End();

			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
		}
	}
}

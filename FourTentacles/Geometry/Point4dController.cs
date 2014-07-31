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
				Pos = value.Xyz;
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

		public IEnumerable<Controller> GetControllers()
		{
			yield return Width;
			foreach (var guide in Guides)
			{
				yield return guide;
				yield return guide.Width;
			}
		}

		public void UpdateGuides(Kompass kompass, SinCosTable table)
		{
			Width.SinCos = table;
			foreach (var guide in Guides)
			{
				Vector3 direction = Pos;
				direction.Normalize();
				Width.Pos = Pos;
				Width.Direction = direction;
				Width.North = kompass.North;
				Width.West = kompass.West;
				guide.Width.Pos = guide.Pos + Pos;
				guide.Width.SinCos = table;
				guide.Width.Direction = direction;
				guide.Width.North = kompass.North;
				guide.Width.West = kompass.West;
			}
		}
	}
}

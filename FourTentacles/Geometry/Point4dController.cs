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

		public Vector4 Point
		{
			get { return new Vector4(Pos, width); }
			set 
			{
				Pos = value.Xyz;
				width = value.W;
			}
		}

		private float width;
		public override void Render(RenderContext context)
		{
			Vector2 pos = context.WorldToScreen(Pos);
			int x = (int) (pos.X - PointSizePx/2.0f);
			int y = (int) (pos.Y - PointSizePx/2.0f);

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

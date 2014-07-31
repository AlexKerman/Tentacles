using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class WidthController : Controller
	{
		public Vector3 Direction;
		public Vector3 North;
		public Vector3 West;
		public SinCosTable SinCos;
		public float Width;

		public override void Render(RenderContext context)
		{
			Material.SetLineMaterial(Color.White);
			GL.Begin(PrimitiveType.LineLoop);
			foreach (Vector3 vec in SinCos.Points(North, West))
				GL.Vertex3(vec * Width + Pos);
			GL.End();
		}
	}
}

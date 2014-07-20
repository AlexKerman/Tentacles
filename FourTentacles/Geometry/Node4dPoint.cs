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
	class Node4DPoint : Controller
	{
		private const int PointSizePx = 3;

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
		public override void DrawContour(Camera camera, Vector3 basePos)
		{
			float size = (float)(camera.GetPerspectiveRatio(Pos + basePos) * PointSizePx / 2.0f);
			var vOffset = camera.Top * size;
			var hOffset = camera.Right * size;

			Material.SetLineMaterial(IsSelected ? Color.Red : Color.White);
			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex3(- hOffset - vOffset);
			GL.Vertex3(  hOffset - vOffset);
			GL.Vertex3(  hOffset + vOffset);
			GL.Vertex3(- hOffset + vOffset);
			GL.End();
		}
	}
}

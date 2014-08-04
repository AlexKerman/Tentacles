using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Guide4DController : Controller
	{
		private readonly Guide4D guide;

		public Guide4DController(Guide4D guide4D)
		{
			guide = guide4D;
		}

		public override Vector3 Pos
		{
			get { return guide.Point.Xyz + guide.BasePoint.Pos; }
			set
			{
				guide.Point = new Vector4(value - guide.BasePoint.Pos, guide.Point.W);
			}
		}

		public override void OnMouseDrag(MouseMoveParams e)
		{
			Pos += e.Constrained;
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
				Vector3 guideSide = Vector3.Cross(guide.BasePoint.WindRose.Dir, context.Camera.VectorToCam(Pos + context.AbsolutePosition));
				guideSide.Normalize();
				Vector3 pointSide = Vector3.Cross(guide.BasePoint.WindRose.Dir, context.Camera.VectorToCam(guide.BasePoint.Pos + context.AbsolutePosition));
				pointSide.Normalize();
				guideSide *= guide.Point.W;
				pointSide *= guide.BasePoint.Point.W;

				GL.Color3(Color.LightYellow);
				GL.Begin(PrimitiveType.Lines);
				
				GL.Vertex3(guide.BasePoint.Pos);
				GL.Vertex3(Pos);

				GL.Vertex3(guide.BasePoint.Pos + pointSide);
				GL.Vertex3(Pos + guideSide);

				GL.Vertex3(guide.BasePoint.Pos - pointSide);
				GL.Vertex3(Pos - guideSide);

				GL.End();
				guide.BasePoint.DrawOrthoPoint(context, Pos);
			}
		}

		public override void OnMouseOver(MouseOverParams mouseOverParams)
		{
			mouseOverParams.Cursor = EditorCursors.Move;
		}
	}
}
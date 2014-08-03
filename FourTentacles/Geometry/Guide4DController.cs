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
				guide.Point.Xyz = value - guide.BasePoint.Pos;
				guide.BasePoint.Changed = true;
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
				GL.Color3(Color.LightYellow);
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(guide.BasePoint.Pos);
				GL.Vertex3(Pos);
				GL.End();
				guide.BasePoint.DrawOrthoPoint(context, Pos);
			}
		}

		public override Cursor GetCursor()
		{
			return EditorCursors.Move;
		}
	}
}
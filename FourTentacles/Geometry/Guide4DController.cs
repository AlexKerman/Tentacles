using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Guide4DController : Controller
	{
		public Point4DController BasePoint;
		public WidthController Width = new WidthController();
		private Vector3 point;

		public Vector4 Point
		{
			get { return new Vector4(point, Width.Width); }
			set
			{
				point = value.Xyz;
				Width.Width = value.W;
			}
		}

		public override Vector3 Pos
		{
			get { return point + BasePoint.Pos; }
			set
			{
				point = value - BasePoint.Pos;
				OnChanged();
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
				Material.SetLineMaterial(Color.LightYellow);
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(BasePoint.Pos);
				GL.Vertex3(Pos + BasePoint.Pos);
				GL.End();
				DrawOrthoPoint(context, BasePoint.Pos + Pos);
			}
		}

		public override Cursor GetCursor()
		{
			return EditorCursors.Move;
		}
	}
}
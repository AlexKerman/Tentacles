using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Guide4DController : Point4DController
	{
		public Point4DController BasePoint;
		private Vector3 pos1;

		//Disable selection
		public override bool IsSelected { get { return false; } set{} }

		public override void OnMouseDrag(MouseMoveParams e)
		{
			Pos += e.Constrained;
		}

		public override void Render(RenderContext context)
		{
			//¬идова€ матрица уже смещена на Pos гайда, но не смещена на Pos базовой точки
			//“ак неправильно, как только начнутс€ повороты объектов, это работать не будет.
			//Ќужно, чтобы гайда опиралась на базовую точку, но ничего про неЄ не знала.
			if (context.Mode == RenderMode.Selection)
			{
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(BasePoint.Pos);
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

		public override Vector3 Pos
		{
			get { return pos1; }
			set
			{
				pos1 = value;
				OnChanged();
			}
		}
	}
}
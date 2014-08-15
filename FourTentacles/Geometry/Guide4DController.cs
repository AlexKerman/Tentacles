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
		}

		private DoUndoGuideMove doUndoMove;
		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			doUndoMove = new DoUndoGuideMove(guide);
			UndoStack.AddAction(doUndoMove);
		}

		public override void OnMouseDrag(MouseMoveParams e)
		{
			doUndoMove.Move(e.Constrained);
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

	class DoUndoGuideMove : IDoUndo
	{
		private readonly Guide4D guide;
		private Vector4 point;
		private Windrose windRose;
		private Vector3 move = Vector3.Zero;

		public DoUndoGuideMove(Guide4D guide)
		{
			this.guide = guide;
			point = guide.Point;
			windRose = guide.BasePoint.WindRose;
		}

		public void Move(Vector3 delta)
		{
			move += delta;
			guide.Point = point + new Vector4(move);
			guide.BasePoint.WindRose.Adjust(Vector3.Normalize(guide.Point.Xyz));
		}

		public void Undo()
		{
			var rose = guide.BasePoint.WindRose;
			guide.Point = point;
			guide.BasePoint.WindRose = windRose;
			windRose = rose;
		}

		public void Redo()
		{
			var rose = guide.BasePoint.WindRose;
			guide.Point = point + new Vector4(move);
			guide.BasePoint.WindRose = windRose;
			windRose = rose;
		}
	}
}
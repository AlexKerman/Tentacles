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
			doUndoMove.Move(new Vector4(e.Constrained));
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
				Vector3 guideSide = Vector3.Cross(guide.WindRose.Dir, context.Camera.VectorToCam(Pos + context.AbsolutePosition));
				guideSide.Normalize();
				Vector3 pointSide = Vector3.Cross(guide.WindRose.Dir, context.Camera.VectorToCam(guide.BasePoint.Pos + context.AbsolutePosition));
				pointSide.Normalize();
				guideSide *= guide.Point.W + guide.BasePoint.Point.W;
				pointSide *= guide.BasePoint.Point.W;

				GL.Color3(Color.DarkGray);
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
		private readonly Vector4 point;
		private Windrose windRose;
		private Vector4 move = Vector4.Zero;

		private readonly Guide4D symGuide;
		private readonly Vector4 symGuidePoint;

		public DoUndoGuideMove(Guide4D guide)
		{
			this.guide = guide;
			point = guide.Point;
			windRose = guide.WindRose;

			symGuide = guide.GetSymmetricGuide();
			if (symGuide != null) symGuidePoint = symGuide.Point;
		}

		public void Move(Vector4 delta)
		{
			move += delta;
			guide.Point = point + move;
			guide.WindRose.Adjust(Vector3.Normalize(guide.Point.Xyz));
			AjustSymmetricGuide();
		}

		public void Undo()
		{
			var rose = guide.WindRose;
			guide.Point = point;
			guide.WindRose = windRose;
			windRose = rose;

			if (symGuide != null) symGuide.Point = symGuidePoint;
		}

		public void Redo()
		{
			var rose = guide.WindRose;
			guide.Point = point + move;
			guide.WindRose = windRose;
			windRose = rose;
		}

		private void AjustSymmetricGuide()
		{
			if (guide.BasePoint.SmoothMode == PointSmoothMode.Cusp) return;
			if (symGuide == null) return;

			if (guide.BasePoint.SmoothMode == PointSmoothMode.Symmetrical) symGuide.Point = -guide.Point;
			if (guide.BasePoint.SmoothMode == PointSmoothMode.Smooth)
				symGuide.Point = -guide.Point * (symGuidePoint.Xyz.Length / guide.Point.Xyz.Length);

			symGuide.WindRose = guide.WindRose;

			//todo: use this prop
			symGuide.Changed = true;
		}
	}
}
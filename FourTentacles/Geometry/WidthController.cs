using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	abstract class WidthController : Controller
	{
		private const int CenterZone = 5;

		protected Point mouseDownLocation;
		private Vector2 circleCenter;

		protected Point4D BasePoint;
		protected Guide4D baseGuide;
		protected float prevWidth;

		private RenderContext lastContext;
		private bool selected;

		public override void OnMouseOver(MouseOverParams mouseOverParams)
		{
			//todo: replace with projection to north-west windrose plane
			int dx = (int) Math.Abs(mouseOverParams.Location.X - circleCenter.X);
			int dy = (int) Math.Abs(mouseOverParams.Location.Y - circleCenter.Y);
			if (dx <= CenterZone && dy > CenterZone)
			{
				mouseOverParams.Cursor = Cursors.SizeNS;
				return;
			}
			if (dy <= CenterZone && dx > CenterZone)
			{
				mouseOverParams.Cursor = Cursors.SizeWE;
				return;
			}
			var sign = (mouseOverParams.Location.X - circleCenter.X)*(mouseOverParams.Location.Y - circleCenter.Y);
			mouseOverParams.Cursor = sign > 0 ? Cursors.SizeNWSE : Cursors.SizeNESW;

			mouseOverParams.Changed = !selected;
			selected = true;
		}

		public override void OnMouseLeave(MouseOverParams mouseOverParams)
		{
			selected = false;
			mouseOverParams.Changed = true;
		}

		public override void OnMouseDrag(MouseMoveParams e)
		{
			var prev = new Vector2(mouseDownLocation.X, mouseDownLocation.Y);
			var loc = new Vector2(e.Location.X, e.Location.Y);
			float baseDist = (prev - circleCenter).Length;
			float newDist = (loc - circleCenter).Length;
			var dot = Vector2.Dot(Vector2.Normalize(prev - circleCenter), Vector2.Normalize(loc - circleCenter));

			float newWidth = prevWidth - baseDist + newDist * dot;
			Width = newWidth;
		}

		public override sealed void Render(RenderContext context)
		{
			lastContext = context;
			circleCenter = context.WorldToScreen(BasePoint.Pos + Offset);
			baseGuide.DrawWidthCircle(context, BasePoint.Pos + Offset, Width, selected);
		}

		public abstract float Width { get; set; }
		public abstract Vector3 Offset { get; }
	}

	class PointWidthController : WidthController
	{
		public PointWidthController(Guide4D guide)
		{
			baseGuide = guide;
			BasePoint = guide.BasePoint;
		}

		DoUndoWidth doUndoWidth;

		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			prevWidth = BasePoint.Point.W;
			doUndoWidth = new DoUndoWidth(BasePoint);
			UndoStack.AddAction(doUndoWidth);
			mouseDownLocation = mouseOverParams.Location;
		}

		public override float Width
		{
			set { doUndoWidth.SetWidth(value); }
			get { return BasePoint.Point.W; }
		}

		public override Vector3 Offset
		{
			get { return Vector3.Zero; }
		}
	}

	class GuideWidthController : WidthController
	{
		public GuideWidthController(Guide4D guide)
		{
			baseGuide = guide;
			BasePoint = guide.BasePoint;
		}

		private DoUndoGuideMove moveGuide;

		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			prevWidth = baseGuide.Point.W;
			moveGuide = new DoUndoGuideMove(baseGuide);
			UndoStack.AddAction(moveGuide);
			mouseDownLocation = mouseOverParams.Location;
		}

		public override float Width
		{
			set { moveGuide.Move(new Vector4(Vector3.Zero, value - baseGuide.Point.W)); }
			get { return baseGuide.Point.W + baseGuide.BasePoint.Point.W; }
		}

		public override Vector3 Offset
		{
			get { return baseGuide.Point.Xyz; }
		}
	}
}

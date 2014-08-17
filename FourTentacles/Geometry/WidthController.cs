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
		private static int centerZone = 5;

		private Point mouseDownLocation;
		protected Vector2 circleCenter;
		protected Point4D BasePoint;
		protected float prevWidth;
		protected RenderContext lastContext;

		protected bool selected;

		public override void OnMouseOver(MouseOverParams mouseOverParams)
		{
			int dx = (int) Math.Abs(mouseOverParams.Location.X - circleCenter.X);
			int dy = (int) Math.Abs(mouseOverParams.Location.Y - circleCenter.Y);
			if (dx <= centerZone && dy > centerZone)
			{
				mouseOverParams.Cursor = Cursors.SizeNS;
				return;
			}
			if (dy <= centerZone && dx > centerZone)
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

		DoUndoWidth doUndoWidth;

		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			doUndoWidth = new DoUndoWidth(this);
			UndoStack.AddAction(doUndoWidth);
			mouseDownLocation = mouseOverParams.Location;
			//BasePoint.WindRose
		}

		public override void OnMouseDrag(MouseMoveParams e)
		{
			var prev = new Vector2(mouseDownLocation.X, mouseDownLocation.Y);
			var loc = new Vector2(e.Location.X, e.Location.Y);
			float baseDist = (prev - circleCenter).Length;
			float newDist = (loc - circleCenter).Length;
			var dot = Vector2.Dot(Vector2.Normalize(prev - circleCenter), Vector2.Normalize(loc - circleCenter));

			float newWidth = prevWidth - baseDist + newDist * dot;
			doUndoWidth.SetWidth(newWidth);
		}

		public override sealed void Render(RenderContext context)
		{
			lastContext = context;
			circleCenter = context.WorldToScreen(BasePoint.Pos + Offset);
			BasePoint.DrawWidthCircle(context, Offset, Width, selected);
		}

		public abstract float Width { get; set; }
		public abstract Vector3 Offset { get; }
	}

	class PointWidthController : WidthController
	{
		public PointWidthController(Point4D point)
		{
			BasePoint = point;
		}

		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			prevWidth = BasePoint.Point.W;
			base.OnMouseDown(mouseOverParams);
		}

		public override float Width
		{
			set { BasePoint.Point = new Vector4(BasePoint.Point.Xyz, value); }
			get { return BasePoint.Point.W; }
		}

		public override Vector3 Offset
		{
			get { return Vector3.Zero; }
		}
	}

	class GuideWidthController : WidthController
	{
		private Guide4D baseGuide = null;

		public GuideWidthController(Guide4D guide)
		{
			baseGuide = guide;
			BasePoint = guide.BasePoint;
		}

		public override void OnMouseDown(MouseOverParams mouseOverParams)
		{
			prevWidth = baseGuide.Point.W;
			base.OnMouseDown(mouseOverParams);
		}

		public override float Width
		{
			set { baseGuide.Point = new Vector4(baseGuide.Point.Xyz, value); }
			get { return baseGuide.Point.W; }
		}

		public override Vector3 Offset
		{
			get { return baseGuide.Point.Xyz; }
		}
	}
}

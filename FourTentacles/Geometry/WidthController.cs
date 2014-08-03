using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
		private Point mouseDownLocation;
		protected Vector2 circleCenter;
		protected Point4D BasePoint;
		protected float prevWidth;

		public override void OnMouseOver(Point location)
		{
			
		}

		public override void OnMouseDown(Point location)
		{
			mouseDownLocation = location;
		}

		public override void OnMouseDrag(MouseMoveParams e)
		{
			//TODO: переписать нахуй
			var prev = new Vector2(mouseDownLocation.X, mouseDownLocation.Y);
			var loc = new Vector2(e.Location.X, e.Location.Y);
			float baseDist = (prev - circleCenter).Length;
			float newDist = (loc - circleCenter).Length;
			float newWidth = prevWidth / baseDist * newDist;
			newWidth *= Math.Sign(e.Location.X - circleCenter.X) * Math.Sign(mouseDownLocation.X - circleCenter.X)
			            * Math.Sign(e.Location.Y - circleCenter.Y) * Math.Sign(mouseDownLocation.Y - circleCenter.Y);
			SetWidth(newWidth);
		}

		protected abstract void SetWidth(float width);
	}

	class PointWidthController : WidthController
	{
		public PointWidthController(Point4D point)
		{
			BasePoint = point;
		}

		public override void Render(RenderContext context)
		{
			circleCenter = context.WorldToScreen(Vector3.Zero);
			BasePoint.DrawWidthCircle(context, new Vector4(Vector3.Zero, BasePoint.Point.W));
		}

		protected override void SetWidth(float width)
		{
			BasePoint.Point = new Vector4(BasePoint.Point.Xyz, width);
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

		public override void Render(RenderContext context)
		{
			circleCenter = context.WorldToScreen(baseGuide.Point.Xyz);
			BasePoint.DrawWidthCircle(context, baseGuide.Point);
		}

		public override void OnMouseDown(Point location)
		{
			prevWidth = baseGuide.Point.W;
			base.OnMouseDown(location);
		}

		public override Cursor GetCursor()
		{
			return Cursors.SizeWE;
		}

		protected override void SetWidth(float width)
		{
			BasePoint.Point = new Vector4(baseGuide.Point.Xyz, width);
		}
	}
}

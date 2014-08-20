using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace FourTentacles
{
	class InsertPointController : Controller
	{
		private readonly Segment4D segment;
		private readonly Spline4D spline;
		private RenderContext lastContext;

		public InsertPointController(Segment4D segment, Spline4D spline)
		{
			this.segment = segment;
			this.spline = spline;
		}

		public override void Render(RenderContext context)
		{
			lastContext = context;
			segment.DrawSpline();
		}

		public override void OnMouseOver(MouseOverParams mouseOverParams)
		{
			mouseOverParams.Cursor = EditorCursors.AddPoint;
		}

		public override void OnMouseDoubleClick(MouseOverParams mouseOverParams)
		{
			float tt = 0;
			float minDist = Single.MaxValue;

			var cursorPoint = new Vector2(mouseOverParams.Location.X, mouseOverParams.Location.Y);
			for (float t = 0; t <= 1; t += 0.001f)
			{
				var p = lastContext.WorldToScreen(segment.GetPoint(t).Xyz);
				var sqDist = (cursorPoint - p).LengthSquared;
				if (sqDist < minDist)
				{
					minDist = sqDist;
					tt = t;
				}
			}

			var midGuide = segment.GetDirection(tt);
			var midPoint = segment.GetPoint(tt);
			var midRose = segment.GetWindrose(tt);

			var p1 = segment.bp;
			var p2 = new Point4D(midPoint, midRose);
			var p3 = segment.ep;

			var g1 = segment.cpbp;
			var g2 = new Guide4D(p2, -midGuide * tt);
			var g3 = new Guide4D(p2, midGuide * (1.0f - tt));
			var g4 = segment.cpep;

			//TODO: undo this action

			g1.Point *= tt;
			g4.Point *= (1.0f - tt);

			segment.ep = p2;
			segment.cpep = g2;

			var segemnt2 = new Segment4D(p2, p3, g3, g4);
			spline.AddSegment(segemnt2);
			segment.Changed = segemnt2.Changed = true;
			mouseOverParams.Changed = true;
		}
	}
}

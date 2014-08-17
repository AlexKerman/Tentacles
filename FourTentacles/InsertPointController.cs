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
		private RenderContext lastContext;

		public InsertPointController(Segment4D segment)
		{
			this.segment = segment;
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
			float tt;
			float minDist = Single.MaxValue;

			var cursorPoint = new Vector2(mouseOverParams.Location.X, mouseOverParams.Location.Y);
			for (float t = 0; t <= 1; t += 0.001f)
			{
				var p = lastContext.WorldToScreen(segment.GetPoint(t).Xyz);
				var sqDist = (cursorPoint - p).LengthSquared;
				if (minDist < sqDist)
				{
					minDist = sqDist;
					tt = t;
				}
			}


		}
	}
}

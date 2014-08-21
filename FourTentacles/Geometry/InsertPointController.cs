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
			var t = GetTFromLocation(mouseOverParams);
			var insertPoint = new DoUndoInsertPoint(t, segment, spline);
			UndoStack.AddAction(insertPoint);

			mouseOverParams.Changed = true;
		}

		private float GetTFromLocation(MouseOverParams mouseOverParams)
		{
			float t = 0;
			float minDist = Single.MaxValue;

			var cursorPoint = new Vector2(mouseOverParams.Location.X, mouseOverParams.Location.Y);
			for (float it = 0; it <= 1; it += 0.001f)
			{
				var p = lastContext.WorldToScreen(segment.GetPoint(it).Xyz);
				var sqDist = (cursorPoint - p).LengthSquared;
				if (sqDist < minDist)
				{
					minDist = sqDist;
					t = it;
				}
			}
			return t;
		}

		class DoUndoInsertPoint :  IDoUndo
		{
			private readonly Spline4D spline;
			private readonly float t;

			//old data
			private readonly Segment4D segment;
			private readonly Vector4 startGuide;
			private readonly Vector4 endGuide;
			
			//new data
			private Segment4D segment2;

			public DoUndoInsertPoint(float t, Segment4D segment, Spline4D spline)
			{
				this.t = t;
				this.segment = segment;
				this.spline = spline;
				startGuide = segment.cpbp.Point;
				endGuide = segment.cpep.Point;

				Redo();
			}

			public void Undo()
			{
				spline.RemoveSegment(segment2);
				segment.ep = segment2.ep;
				segment.cpbp.Point = startGuide;
				segment.cpep.Point = endGuide;
				segment.Changed = true;
			}

			public void Redo()
			{
				var midGuide = segment.GetDirection(t) / 3.0f;
				var midPoint = segment.GetPoint(t);
				var midRose = segment.GetWindrose(t);

				var addedPoint = new Point4D(midPoint, midRose);

				var g1 = segment.cpbp;
				var g2 = new Guide4D(addedPoint, -midGuide * t);
				var g3 = new Guide4D(addedPoint, midGuide * (1.0f - t));
				var g4 = segment.cpep;

				g1.Point *= t;
				g4.Point *= (1.0f - t);

				addedPoint.Guides.Add(g2);

				segment2 = new Segment4D(addedPoint, segment.ep, g3, g4);
				spline.AddSegment(segment2);
				segment.ep = addedPoint;
				segment.cpep = g2;
				segment.Changed = segment2.Changed = true;
			}
		}
	}
}

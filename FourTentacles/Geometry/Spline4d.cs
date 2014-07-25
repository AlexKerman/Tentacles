using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourTentacles.Annotations;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Spline4D : Geometry
	{
		public enum SelectionModeEnum
		{
			None, Points, Segments
		}

		public SelectionModeEnum SelectionMode = SelectionModeEnum.None;

		private int roundSides;
		private int lengthSides;
		private SinCosTable sinCos;
		private List<Segment4D> segments = new List<Segment4D>();
		private List<Node4DPoint> points = new List<Node4DPoint>();

		public Spline4D(int roundSides, int lengthSides)
		{
			this.roundSides = roundSides;
			this.lengthSides = lengthSides;
			sinCos = new SinCosTable(roundSides);
		}

		public int RoundSides
		{
			get { return roundSides; }
			set
			{
				if(roundSides == value || value < 2) return;
				roundSides = value;
				sinCos = new SinCosTable(roundSides);
				RecalculateGeometry();
			}
		}

		public int LengthSides
		{
			get { return lengthSides; }
			set
			{
				if(lengthSides == value || value < 2) return;
				lengthSides = value;
				RecalculateGeometry();
			}
		}

		private void RecalculateGeometry()
		{
			foreach (var segment in segments)
				segment.CalculateGeometry(sinCos, lengthSides);
		}

		public void AddSegment(Node4DPoint start, Node4DPoint end, Vector4 startGuide, Vector4 endGuide)
		{
			var segment = new Segment4D(start, end, startGuide, endGuide);
			segment.CalculateGeometry(sinCos, lengthSides);
			segments.Add(segment);
			points.Add(start);
			points.Add(end);
		}

		override public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var segment in segments)
				bb = bb.Extend(segment.GetBoundingBox());
			return bb.Translate(Pos);
		}

		override public void Render(RenderMode renderMode)
		{
			foreach (var segment in segments)
				segment.Render(renderMode);
		}

		override public int GetTrianglesCount()
		{
			return segments.Sum(s => s.GetTrianglesCount());
		}

		public override GeometryControl GetNodeControl()
		{
			return new Spline4dControl(this);
		}

		public override IEnumerable<Controller> GetControllers()
		{
			return points;
		}

		public override bool HasSelectedNodes()
		{
			return points.Any(p => p.IsSelected);
		}

		public override IEnumerable<Node> GetNodes()
		{
			if(SelectionMode == SelectionModeEnum.Points) return points;
			if (SelectionMode == SelectionModeEnum.Segments) return segments;
			return new Node[0];
		}

		public override void Move(Vector3 vector)
		{
			if (SelectionMode == SelectionModeEnum.None)
			{
				Pos += vector;
				return;
			}

			if(SelectionMode == SelectionModeEnum.Points && points.Any(p => p.IsSelected))
			{
				foreach (var point in points.Where(p => p.IsSelected))
				{
					point.Pos += vector;
				}
				RecalculateGeometry();
			}
		}
	}
}

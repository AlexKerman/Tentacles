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

		private SinCosTable sinCos;
		private List<Segment4D> segments = new List<Segment4D>();

		private bool changed = true;

		public Spline4D(int roundSides, int lengthSides)
		{
			this.roundSides = roundSides;
			this.lengthSides = lengthSides;
			sinCos = new SinCosTable(roundSides);
		}

		#region geometry params

		private int roundSides;
		private int lengthSides;
		private bool lengthSmooth = true;

		public int RoundSides
		{
			get { return roundSides; }
			set
			{
				if(roundSides == value || value < 2) return;
				roundSides = value;
				sinCos = new SinCosTable(roundSides);
				changed = true;
			}
		}

		public int LengthSides
		{
			get { return lengthSides; }
			set
			{
				if(lengthSides == value || value < 2) return;
				lengthSides = value;
				changed = true;
			}
		}

		public bool LengthSmooth
		{
			get { return lengthSmooth; }
			set
			{
				if(lengthSmooth == value) return;
				lengthSmooth = value;
				changed = true;
			}
		}

		#endregion eometry params

		public void AddSegment(Segment4D segment)
		{
			segments.Add(segment);
			segment.CalculateGeometry(SmoothAlgorithm(), sinCos, lengthSides);
		}

		public void RemoveSegment(Segment4D segment)
		{
			segments.Remove(segment);
		}

		override public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var segment in segments)
				bb = bb.Extend(segment.GetBoundingBox());
			return bb.Translate(Pos);
		}

		private Mesh SmoothAlgorithm()
		{
			return lengthSmooth ? (Mesh) new SmoothMesh() : new SmoothLengthMesh();
		}

		override public void Render(RenderContext context)
		{
			foreach (var segment in segments)
			{
				if(changed || segment.Changed) segment.CalculateGeometry(SmoothAlgorithm(), sinCos, lengthSides);
				segment.Render(context);
			}
			foreach (var node in GetNodes()) node.Render(context);
			foreach (var controller in GetControllers()) controller.Render(context);

			changed = false;
			foreach (var segment in segments)
				segment.Changed = false;
		}

		override public int GetTrianglesCount()
		{
			return segments.Sum(s => s.GetTrianglesCount());
		}

		public override GeometryControl GetNodeControl()
		{
			return new Spline4dControl(this);
		}

		public override IEnumerable<Node> GetNodes()
		{
			if (SelectionMode == SelectionModeEnum.Points) return GetPoints();
			if (SelectionMode == SelectionModeEnum.Segments) return segments;
			return new Node[0];
		}

		private IEnumerable<Point4D> GetPoints()
		{
			var points = new HashSet<Point4D>();
			foreach (var segment in segments)
			{
				if (!points.Contains(segment.bp)) points.Add(segment.bp);
				if (!points.Contains(segment.ep)) points.Add(segment.ep);
			}
			return points;
		}

		public override IEnumerable<Controller> GetControllers()
		{
			if(SelectionMode == SelectionModeEnum.Points)
			{
				foreach (var point in GetPoints())
					if(point.IsSelected)
						foreach (var guide in point.Guides)
							foreach (var controller in guide.GetControllers())
								yield return controller;
				foreach (var segment in segments)
					yield return new InsertPointController(segment, this);
			}
		}
	}
}

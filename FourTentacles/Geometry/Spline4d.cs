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
		private List<Point4D> points = new List<Point4D>();

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
				foreach (var segment in segments)
					segment.Mesh = lengthSmooth ? (Mesh)new SmoothMesh() : new SmoothLengthMesh();
				changed = true;
			}
		}

		#endregion eometry params

		public void AddSegment(Vector4 start, Vector4 end, Vector4 startGuide, Vector4 endGuide)
		{
			var startPoint = new Point4D(start, startGuide.Xyz);
			var endPoint = new Point4D(end, -endGuide.Xyz);
			var segment = new Segment4D(startPoint, endPoint, new Guide4D(startPoint, startGuide), new Guide4D(endPoint, endGuide));
			segment.CalculateGeometry(sinCos, lengthSides);
			segments.Add(segment);
			points.Add(startPoint);
			points.Add(endPoint);
		}

		override public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var segment in segments)
				bb = bb.Extend(segment.GetBoundingBox());
			return bb.Translate(Pos);
		}

		override public void Render(RenderContext context)
		{
			foreach (var segment in segments)
			{
				if(changed || segment.Changed) segment.CalculateGeometry(sinCos, lengthSides);
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
			if (SelectionMode == SelectionModeEnum.Points) return points;
			if (SelectionMode == SelectionModeEnum.Segments) return segments;
			return new Node[0];
		}

		public override IEnumerable<Controller> GetControllers()
		{
			if(SelectionMode == SelectionModeEnum.Points)
				foreach (var point in points)
					if(point.IsSelected)
						foreach (var controller in point.GetControllers())
							yield return controller;
		}
	}
}

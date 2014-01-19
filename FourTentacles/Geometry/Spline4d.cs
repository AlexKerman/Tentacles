using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Spline4D : Node, ISelectable
	{
		private readonly int roundSides;
		private readonly int lenghtSides;
		private List<Segment4D> segments = new List<Segment4D>();

		public Spline4D(int roundSides, int lenghtSides)
		{
			this.roundSides = roundSides;
			this.lenghtSides = lenghtSides;
		}

		public void AddSegment(Vector4 start, Vector4 end, Vector4 startGuide, Vector4 endGuide)
		{
			segments.Add(new Segment4D(start, end, startGuide, endGuide, roundSides, lenghtSides));
		}

		public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var segment in segments)
				bb = bb.Extend(segment.GetBoundingBox());
			return bb.Translate(Pos);
		}

		override public void Render(RenderMode renderMode)
		{
			foreach (var segment in segments)
				segment.Mesh.Render(renderMode);
		}

		public void DrawShape()
		{
			foreach (var segment in segments)
				segment.Mesh.Render(RenderMode.Solid);
		}

		public int GetTrianglesCount()
		{
			return segments.Sum(s => s.Mesh.GetTrianglesCount());
		}
	}
}

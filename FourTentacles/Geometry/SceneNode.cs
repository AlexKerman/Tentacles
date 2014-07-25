using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourTentacles.Annotations;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class SceneNode : Geometry
	{
		public event EventHandler RedrawRequired;

		private List<Geometry> geometrys = new List<Geometry>();
		private Geometry lockedGeometry;
		private GeometryControl lockedControl;

		public SceneNode()
		{
			var spline = new Spline4D(4, 96);
			var bp = new Node4DPoint {Point = new Vector4(0.0f, 200.0f, 0.0f, 50.0f)};
			var ep = new Node4DPoint {Point = new Vector4(0.0f, 1200.0f, -200.0f, 0.0f)};
			spline.AddSegment(bp, ep,
				new Vector4(0.0f, 800.0f, 0.0f, -150.0f),
				new Vector4(0.0f, 600.0f, 800.0f, 200.0f));
			geometrys.Add(spline);
		}
		
		public override GeometryControl GetNodeControl()
		{
			if (lockedGeometry != null) return lockedControl;

			var selectedNodes = geometrys.Where(g => g.IsSelected).ToList();
			if (selectedNodes.Count == 1)
			{
				lockedControl = selectedNodes[0].GetNodeControl();
				if (lockedControl != null)
				{
					lockedControl.GeometryLocked += ControlOnGeometryLocked;
					lockedControl.GeometryUnlocked += ControlOnGeometryUnlocked;
					lockedControl.RedrawRequired += LockedControlOnRedrawRequired;
					return lockedControl;
				}
			}
			return null;
		}

		private void LockedControlOnRedrawRequired(object sender, EventArgs eventArgs)
		{
			RedrawRequired.Raise();
		}

		private void ControlOnGeometryUnlocked(object sender, GeometryControl.GeometryLockedEventArgs e)
		{
			lockedGeometry = null;
			RedrawRequired.Raise();
		}

		private void ControlOnGeometryLocked(object sender, GeometryControl.GeometryLockedEventArgs e)
		{
			lockedGeometry = e.geometry;
			RedrawRequired.Raise();
		}

		public override void Render(RenderMode renderMode)
		{
			foreach (var geom in geometrys)
			{
				GL.PushMatrix();
				GL.Translate(geom.Pos);
				geom.Render(renderMode);
				GL.PopMatrix();
			}
		}

		public void DrawContour(Camera camera)
		{
			if (lockedGeometry != null)
				foreach (var controller in lockedGeometry.GetControllers())
				{
					GL.PushMatrix();
					GL.Translate(controller.Pos);
					controller.DrawContour(camera, lockedGeometry.Pos);
					GL.PopMatrix();
				}
		}

		public override void Move(Vector3 vector)
		{
			foreach (var geometry in geometrys)
				if (geometry.IsSelected)
					geometry.Move(vector);
		}

		public override BoundingBox GetBoundingBox()
		{
			var box = new BoundingBox();
			if (lockedGeometry != null) return box.Extend(lockedGeometry.GetBoundingBox());
			foreach (var geo in geometrys.Where(g => g.IsSelected))
				box = box.Extend(geo.GetBoundingBox());
			return box;
		}

		public override int GetTrianglesCount()
		{
			return geometrys.Sum(n => n.GetTrianglesCount());
		}

		public override IEnumerable<Node> GetNodes()
		{
			if (lockedGeometry != null) return lockedGeometry.GetNodes();
			return geometrys;
		}

		public Vector3 GetNodesPos()
		{
			if (lockedGeometry != null)
				return lockedGeometry.Pos;
			return Vector3.Zero;
		}

		public override bool HasSelectedNodes()
		{
			return GetNodes().Any(g => g.IsSelected);
		}
	}
}

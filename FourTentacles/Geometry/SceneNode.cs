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
			var spline = new Spline4D(6, 96);
			var startPoint = new Point4D(new Vector4(0.0f, 200.0f, 0.0f, 50.0f));
			var endPoint = new Point4D(new Vector4(0.0f, 1200.0f, -200.0f, 0.0f));

			var startGuide = new Guide4D(startPoint, new Vector4(0.0f, 800.0f, 0.0f, -150.0f));
			var endGuide =new Guide4D(endPoint, new Vector4(0.0f, 600.0f, 800.0f, 200.0f));

			var segment = new Segment4D(startPoint, endPoint, startGuide, endGuide);
			spline.AddSegment(segment);
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

		public override void Render(RenderContext context)
		{
			foreach (var geom in geometrys)
			{
				GL.PushMatrix();
				GL.Translate(geom.Pos);
				geom.Render(context);
				GL.PopMatrix();
			}
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

		public override IEnumerable<Controller> GetControllers()
		{
			if (lockedGeometry != null)
				return lockedGeometry.GetControllers();
			return new Controller[0];
		}

		public Vector3 GetNodesPos()
		{
			if (lockedGeometry != null)
				return lockedGeometry.Pos;
			return Vector3.Zero;
		}

		public virtual bool HasSelectedNodes()
		{
			return GetNodes().Any(g => g.IsSelected);
		}
	}
}

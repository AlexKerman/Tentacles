using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	public abstract class Geometry : Node
	{
		public abstract BoundingBox GetBoundingBox();
		public abstract int GetTrianglesCount();
		public abstract void Render(RenderMode renderMode);
		public virtual IEnumerable<Controller> GetControllers() { yield break; }

		public override void DrawContour(Camera camera, Vector3 basePos)
		{
			Render(RenderMode.Solid);
		}

		public virtual bool HasSelectedNodes()
		{
			return false;
		}

		public virtual IEnumerable<Node> GetNodes()
		{
			yield break;
		}

		public virtual void Move(Vector3 vector)
		{
		}
	}
}

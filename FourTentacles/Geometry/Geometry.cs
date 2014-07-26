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
		public virtual IEnumerable<Controller> GetControllers() { yield break; }

		public virtual IEnumerable<Node> GetNodes()
		{
			yield break;
		}

		public virtual void Move(Vector3 vector)
		{
		}
	}
}

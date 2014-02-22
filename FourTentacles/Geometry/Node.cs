using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	public abstract class Node : ISelectable
	{
		virtual public Vector3 Pos { get; set; }
		public abstract void Render(RenderMode renderMode);
		public void DrawShape()
		{
			Render(RenderMode.Solid);
		}

		public abstract BoundingBox GetBoundingBox();
		public abstract int GetTrianglesCount();
	}
}

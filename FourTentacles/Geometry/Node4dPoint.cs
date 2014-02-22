using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	class Node4DPoint : Node
	{
		public Vector4d Point { get; set; }

		public override void Render(RenderMode renderMode)
		{
			throw new NotImplementedException();
		}

		public override BoundingBox GetBoundingBox()
		{
			throw new NotImplementedException();
		}

		public override int GetTrianglesCount()
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	struct BoundingBox
	{
		private Vector3 minCoords;
		private Vector3 maxCoords;
		private bool initialized;

		private const float LineSize = 0.1f;

		public Vector3 Center
		{
			get { return (minCoords + maxCoords)/2.0f; }
		}

		public bool Initialized { get { return initialized; } }

		private BoundingBox(Vector3 minPoint, Vector3 maxPoint)
		{
			minCoords = minPoint;
			maxCoords = maxPoint;
			initialized = true;
		}

		public BoundingBox Extend(Vector3 point)
		{
			if (initialized)
				return new BoundingBox(Vector3.ComponentMin(minCoords, point), Vector3.ComponentMax(maxCoords, point));
			return new BoundingBox(point, point);
		}

		public BoundingBox Extend(BoundingBox otherBox)
		{
			if (!otherBox.initialized) return this;
			if (initialized)
				return new BoundingBox(Vector3.ComponentMin(minCoords, otherBox.minCoords), 
					Vector3.ComponentMax(maxCoords, otherBox.maxCoords));
			return otherBox;
		}

		public void Draw()
		{
			Material.SetLineMaterial(Color.White);
			Vector3 size = maxCoords - minCoords;
			GL.PushMatrix();
			GL.Translate(minCoords);
			GL.Scale(size);
			if (size.X > float.Epsilon) DrawOneSide(Vector3.UnitY, Vector3.UnitZ, Vector3.UnitX);
			if (size.Y > float.Epsilon) DrawOneSide(Vector3.UnitX, Vector3.UnitZ, Vector3.UnitY);
			if (size.Z > float.Epsilon) DrawOneSide(Vector3.UnitY, Vector3.UnitX, Vector3.UnitZ);
			GL.PopMatrix();
		}

		private void DrawOneSide(Vector3 a, Vector3 b, Vector3 c)
		{
			GL.Begin(PrimitiveType.Lines);
			foreach (Vector3 vec in new[]{a,b,a+b, Vector3.Zero})
			{
				GL.Vertex3(vec);
				GL.Vertex3(vec + c * LineSize);
				GL.Vertex3(vec + c * (1.0f - LineSize));
				GL.Vertex3(vec + c);
			}
			GL.End();
		}

		public BoundingBox Translate(Vector3 pos)
		{
			return new BoundingBox(minCoords + pos, maxCoords + pos);
		}
	}
}

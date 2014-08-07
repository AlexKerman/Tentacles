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
	abstract class Mesh
	{
		protected Vector3[] pointsArray;
        protected Vector3[] normalsArray;
        protected List<int[]> triangleStripIndicies = new List<int[]>();
        protected List<int[]> lineStripIndicies = new List<int[]>();

	    public abstract void Init(Vector3[] points, Vector3[] normals, int length, int sides);

		public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var point in pointsArray)
				bb = bb.Extend(point);
			return bb;
		}

		public void Render(RenderMode renderMode)
		{
			int stride = BlittableValueType.StrideOf(pointsArray);

			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);
			GL.VertexPointer(3, VertexPointerType.Float, stride, pointsArray);
			GL.NormalPointer(NormalPointerType.Float, stride, normalsArray);

			Material.SetMeshMaterial();
			if (renderMode.HasFlag(RenderMode.Solid) || renderMode == RenderMode.Selection)
				foreach (int[] indicies in triangleStripIndicies)
					GL.DrawElements(PrimitiveType.TriangleStrip, indicies.Length, DrawElementsType.UnsignedInt, indicies);

			GL.DisableClientState(ArrayCap.NormalArray);

			Material.SetLineMaterial(Color.White);
			if (renderMode.HasFlag(RenderMode.Wireframe))
			{
				foreach (int[] indicies in triangleStripIndicies)
					GL.DrawElements(PrimitiveType.Lines, indicies.Length, DrawElementsType.UnsignedInt, indicies);

				foreach (int[] indicies in lineStripIndicies)
					GL.DrawElements(PrimitiveType.LineStrip, indicies.Length, DrawElementsType.UnsignedInt, indicies);
			}

			if (renderMode.HasFlag(RenderMode.Normals))
			{
				Material.SetLineMaterial(Color.Yellow);
				GL.Begin(PrimitiveType.Lines);
				for (int i = 0; i < pointsArray.Length; i++)
				{
					Vector3 point = pointsArray[i];
					Vector3 normal = normalsArray[i] * 50;
					GL.Vertex3(point);
					GL.Vertex3(normal + point);
				}
				GL.End();
			}
			GL.DisableClientState(ArrayCap.VertexArray);
		}

		public int GetTrianglesCount()
		{
			return triangleStripIndicies.Sum(s => s.Length - 2);
		}
	}

    class SmoothMesh : Mesh
    {
        public override void Init(Vector3[] points, Vector3[] normals, int length, int sides)
        {
            pointsArray = points;
            normalsArray = normals;
			triangleStripIndicies.Clear();
			lineStripIndicies.Clear();

            for (int ti = 0; ti < length - 1; ti++)
            {
                int offset = ti * sides;
                var indicies = new int[sides * 2 + 2];
                for (int i = 0; i < sides; i++)
                {
                    indicies[i * 2] = i + offset;
                    indicies[i * 2 + 1] = i + sides + offset;
                }
                indicies[sides * 2] = offset;
                indicies[sides * 2 + 1] = offset + sides;
                triangleStripIndicies.Add(indicies);

                var lineIndicies = new int[sides + 1];
                for (int i = 0; i < sides; i++)
                    lineIndicies[i] = i + offset;
                lineIndicies[sides] = offset;
                lineStripIndicies.Add(lineIndicies);
            }
        }
    }

    class SmoothLengthMesh : Mesh
    {
        public override void Init(Vector3[] points, Vector3[] normals, int length, int sides)
        {
            pointsArray = new Vector3[points.Length * 2];
            normalsArray = new Vector3[normals.Length * 2];
			triangleStripIndicies.Clear();
			lineStripIndicies.Clear();

            int currentPos = 0;
            for (int i = 0; i < sides; i++)
            {
                int ptOffset = (i + 1)%sides;
                var indicies = new List<int>();
				var lineIndicies = new int[length];
                for (int j = 0; j < length; j++)
                {
	                lineIndicies[j] = currentPos;
                    Vector3 normal = normals[j*sides + i] + normals[j*sides + ptOffset];
                    normal.Normalize();

                    pointsArray[currentPos] = points[j*sides + i];
                    pointsArray[currentPos + 1] = points[j*sides + ptOffset];
                    normalsArray[currentPos] = normal;
                    normalsArray[currentPos + 1] = normal;
                    indicies.Add(currentPos);
                    indicies.Add(currentPos + 1);
                    currentPos += 2;
                }
				lineStripIndicies.Add(lineIndicies);
                triangleStripIndicies.Add(indicies.ToArray());
            }
        }
    }
}

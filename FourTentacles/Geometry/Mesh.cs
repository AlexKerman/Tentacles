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
	class Mesh
	{
		private Vector3[] pointsArray;
		private Vector3[] normalsArray;
		private List<int[]> triangleStripIndicies = new List<int[]>();
		private List<int[]> lineStripIndicies = new List<int[]>();

		private int nextIndex = 0;
		public Mesh(int points)
		{
			pointsArray = new Vector3[points];
			normalsArray = new Vector3[points];
		}

		public BoundingBox GetBoundingBox()
		{
			var bb = new BoundingBox();
			foreach (var point in pointsArray)
				bb = bb.Extend(point);
			return bb;
		}

		public void AddPoint(Vector3 point, Vector3 normal)
		{
			if(nextIndex == pointsArray.Length) throw new Exception("Points array full");
			pointsArray[nextIndex] = point;
			normalsArray[nextIndex] = normal;
			nextIndex++;
		}

		public void AddTriangleStripIndicies(int[] indicies)
		{
			if(nextIndex != pointsArray.Length) throw new Exception("Points array didn't filled comletely");

			triangleStripIndicies.Add(indicies);
		}

		public void AddLineStripIndicies(int[] lineIndicies)
		{
			lineStripIndicies.Add(lineIndicies);
		}

		public void Render(RenderMode renderMode)
		{
			//int stride = BlittableValueType.StrideOf(pointsArray);

			//GL.Enable(EnableCap.VertexArray);
			//GL.Enable(EnableCap.NormalArray);
			//GL.VertexPointer(3, VertexPointerType.Float, stride, pointsArray);
			//GL.NormalPointer(NormalPointerType.Float, stride, normalsArray);
			
			Material.SetMeshMaterial();
			if (renderMode.HasFlag(RenderMode.Solid))
				foreach (int[] indicies in triangleStripIndicies)
					//GL.DrawElements(PrimitiveType.TriangleStrip, indicies.Length, DrawElementsType.UnsignedInt, indicies);
					WorkAroundDrawElements(PrimitiveType.TriangleStrip, pointsArray, normalsArray, indicies);

			//GL.Disable(EnableCap.NormalArray);
			
			Material.SetLineMaterial(Color.White);
			if (renderMode.HasFlag(RenderMode.Wireframe))
			{
				foreach (int[] indicies in triangleStripIndicies)
					//GL.DrawElements(PrimitiveType.Lines, indicies.Length, DrawElementsType.UnsignedInt, indicies);
					WorkAroundDrawElements(PrimitiveType.Lines, pointsArray, indicies);

				foreach (int[] indicies in lineStripIndicies)
					//GL.DrawElements(PrimitiveType.LineStrip, indicies.Length, DrawElementsType.UnsignedInt, indicies);
					WorkAroundDrawElements(PrimitiveType.LineStrip, pointsArray, indicies);
			}

			if (renderMode.HasFlag(RenderMode.Normals))
			{
				Material.SetLineMaterial(Color.Yellow);
				GL.Begin(PrimitiveType.Lines);
				for (int i = 0; i < pointsArray.Length; i++)
				{
					Vector3 point = pointsArray[i];
					Vector3 normal = normalsArray[i]*50;
					GL.Vertex3(point);
					GL.Vertex3(normal + point);
				}
				GL.End();
			}

			//GL.Disable(EnableCap.VertexArray);
			
		}

		private void WorkAroundDrawElements(PrimitiveType primitiveType, Vector3[] pointsArray, int[] indicies)
		{
			GL.Begin(primitiveType);
			foreach (var i in indicies)
			{
				GL.Vertex3(pointsArray[i]);
			}
			GL.End();
		}

		private void WorkAroundDrawElements(PrimitiveType primitiveType, Vector3[] pointsArray, Vector3[] normalsArray, int[] indicies)
		{
			GL.Begin(primitiveType);
			foreach (var i in indicies)
			{
				GL.Normal3(normalsArray[i]);
				GL.Vertex3(pointsArray[i]);
			}
			GL.End();
		}

		public int GetTrianglesCount()
		{
			return triangleStripIndicies.Sum(s => s.Length - 2);
		}
	}
}

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
	class Gizmo
	{
		private const int GizmoSizePx = 64;
		private const int ArrowSides = 6;
		private const float ArrowSize = 0.3f;
		private const float ArrowWidth = 0.05f;
		private const float QuadSize = 0.3f;
		private const float SignSize = 0.07f;
		private readonly static Color SelectedColor = Color.Yellow;

		private SinCosTable sinCos;
		private bool[] constraints = new[] {true, true, false};

		private static readonly Color[] AxisColors = new[] {Color.DarkRed, Color.DarkGreen, Color.DarkBlue};
		private static readonly Vector3[] AxisVectors = new[] {Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ};

		private static readonly Vector2[][] AxisSigns = new[]
			{
				//X
				new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0.6f,0), new Vector2(0,1)},
				//Y
				new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0,1), new Vector2(0.3f,0.5f)},
				//Z
				new[] {new Vector2(0,1), new Vector2(0.6f,1), new Vector2(0.6f,1), new Vector2(0,0), new Vector2(0,0), new Vector2(0.6f,0)}
			};

		public Gizmo()
		{
			sinCos = new SinCosTable(ArrowSides);
		}

		public void Draw(Vector3 gizmoPos, Camera camera, Size controlSize)
		{
			float scale = (float) camera.GetPerspectiveRatio(gizmoPos)*GizmoSizePx/controlSize.Height;

			Vector3 signAxisX = camera.Right * SignSize;
			Vector3 signAxisY = camera.Top * SignSize;

			GL.PushMatrix();
			GL.Translate(gizmoPos);
			GL.Scale(scale, scale, scale);

			for (int axis = 0; axis < 3; axis++)
			{
				int axis2 = axis + 1;
				if (axis2 > 2) axis2 -= 3;
				int axis3 = axis + 2;
				if (axis3 > 2) axis3 -= 3;

				GL.Begin(BeginMode.Lines);
				GL.Color3(constraints[axis] && constraints[axis2]? SelectedColor : AxisColors[axis]);
				GL.Vertex3(QuadSize * AxisVectors[axis]);
				GL.Vertex3(QuadSize * AxisVectors[axis] + QuadSize * AxisVectors[axis2]);
				GL.Color3(constraints[axis] && constraints[axis3] ? SelectedColor : AxisColors[axis]);
				GL.Vertex3(QuadSize * AxisVectors[axis]);
				GL.Vertex3(QuadSize * AxisVectors[axis] + QuadSize * AxisVectors[axis3]);
				GL.Color3(constraints[axis] ? SelectedColor : AxisColors[axis]);
				GL.Vertex3(Vector3.Zero);
				GL.Vertex3(AxisVectors[axis]);

				foreach (Vector2 sign in AxisSigns[axis])
					GL.Vertex3(AxisVectors[axis] + ((sign.X + 1.0f) * signAxisX) + ((sign.Y + 1.0f) * signAxisY));
				GL.End();

				GL.Color3(AxisColors[axis]);
				GL.Begin(BeginMode.TriangleFan);
				GL.Vertex3(AxisVectors[axis]);
				for (int i = 0; i <= ArrowSides; i++)
				{
					Vector3 ringPoint = sinCos.RingPoint(AxisVectors[axis2], AxisVectors[axis3], i);
					GL.Vertex3((1.0f - ArrowSize) * AxisVectors[axis] + ArrowWidth * ringPoint);
				}
				GL.End();
			}

			GL.PopMatrix();
		}
	}
}

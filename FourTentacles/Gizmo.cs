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
		[Flags]
		enum Constraints
		{
			X = 1,
			Y = 2,
			Z = 4,
		}

		class Plane : Controller
		{
			private static readonly Color color = Color.FromArgb(168, 255, 255, 0);

			private readonly Vector3 axis1;
			private readonly Vector3 axis2;
			private readonly Constraints constraint;

			public Constraints Constraint { get { return constraint; } }

			public Plane(Vector3 axis1, Vector3 axis2, Constraints constraint)
			{
				this.axis1 = axis1;
				this.axis2 = axis2;
				this.constraint = constraint;
			}

			private Vector3 pos;
			private float scale;

			public void AdjustPosition(float scale, Vector3 pos)
			{
				this.scale = scale;
				this.pos = pos;
			}

			public void Draw(Constraints constraint)
			{
				if(constraint != this.constraint) return;
				GL.Color4(color);
				GL.Begin(PrimitiveType.TriangleStrip);
				GL.Vertex3(Vector3.Zero);
				GL.Vertex3(axis1 * QuadSize);
				GL.Vertex3(axis2 * QuadSize);
				GL.Vertex3((axis1 + axis2) * QuadSize);
				GL.End();
			}

			public override void DrawShape()
			{
				GL.Begin(PrimitiveType.Triangles);
				GL.Vertex3(axis1 * QuadSize * scale + pos);
				GL.Vertex3(axis2 * QuadSize * scale + pos);
				GL.Vertex3((axis1 + axis2) * QuadSize * scale + pos);
				GL.End();
			}
		}

		class Axis : Controller
		{
			private const int ArrowSides = 6;
			private const float ArrowSize = 0.3f;
			private const float ArrowWidth = 0.05f;

			private const float SignSize = 0.07f;

			private SinCosTable sinCos = new SinCosTable(ArrowSides);
			private readonly Color color;
			private readonly Vector3 axisVector;
			private readonly Constraints constraint;
			private readonly Vector2[] sign;
			private readonly static Color SelectedColor = Color.Yellow;

			public Constraints Constraint { get { return constraint; } }

			public Axis(Color color, Vector3 axisVector, Constraints constraint, Vector2[] sign)
			{
				this.color = color;
				this.axisVector = axisVector;
				this.constraint = constraint;
				this.sign = sign;
			}

			private Vector3 pos;
			private float scale;

			public void AdjustPosition(float scale, Vector3 pos)
			{
				this.scale = scale;
				this.pos = pos;
			}

			public override void DrawShape()
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(pos + axisVector * scale * QuadSize);
				GL.Vertex3(pos + axisVector * scale);
				GL.End();
			}

			public void Draw(Axis axis1, Axis axis2, Constraints constraints, Camera camera)
			{
				Vector3 signAxisX = camera.Right * SignSize;
				Vector3 signAxisY = camera.Top * SignSize;

				GL.Begin(PrimitiveType.Lines);

				foreach (var axis in new[] {axis1, axis2})
				{
					GL.Color3(constraints.HasFlag(constraint | axis.constraint) ? SelectedColor : color);
					GL.Vertex3(QuadSize * axisVector);
					GL.Vertex3(QuadSize * axisVector + QuadSize * axis.axisVector);
				}

				GL.Color3(constraints.HasFlag(constraint) ? SelectedColor : color);
				GL.Vertex3(Vector3.Zero);
				GL.Vertex3(axisVector);

				foreach (Vector2 point in sign)
					GL.Vertex3(axisVector + ((point.X + 1.0f) * signAxisX) + ((point.Y + 1.0f) * signAxisY));
				GL.End();

				GL.Color3(color);
				GL.Begin(PrimitiveType.TriangleFan);
				GL.Vertex3(axisVector);
				for (int i = 0; i <= ArrowSides; i++)
				{
					Vector3 ringPoint = sinCos.RingPoint(axis1.axisVector, axis2.axisVector, i);
					GL.Vertex3((1.0f - ArrowSize) * axisVector + ArrowWidth * ringPoint);
				}
				GL.End();
			}
		}

		private const float QuadSize = 0.3f;
		private const int GizmoSizePx = 96;
		private Constraints fixedConstraints = Constraints.X | Constraints.Y;
		private Constraints constraints = Constraints.X | Constraints.Y;

		private Axis AxisX = new Axis(Color.DarkRed, Vector3.UnitX, Constraints.X, new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0.6f,0), new Vector2(0,1)});
		private Axis AxisY = new Axis(Color.DarkGreen, Vector3.UnitY, Constraints.Y, new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0,1), new Vector2(0.3f,0.5f)});
		private Axis AxisZ = new Axis(Color.DarkBlue, Vector3.UnitZ, Constraints.Z, new[] {new Vector2(0,1), new Vector2(0.6f,1), new Vector2(0.6f,1), new Vector2(0,0), new Vector2(0,0), new Vector2(0.6f,0)});
		private Plane PlaneXY = new Plane(Vector3.UnitX, Vector3.UnitY, Constraints.X | Constraints.Y);
		private Plane PlaneYZ = new Plane(Vector3.UnitY, Vector3.UnitZ, Constraints.Y | Constraints.Z);
		private Plane PlaneZX = new Plane(Vector3.UnitZ, Vector3.UnitX, Constraints.Z | Constraints.X);

		public event EventHandler ViewChanged;

		public Gizmo()
		{
			foreach (var axis in new[] {AxisX, AxisY, AxisZ})
			{
				axis.MouseDown += OnMouseDown;
				axis.MouseLeave += OnMouseLeave;
				axis.MouseOver += OnMouseOverAxis;
			}
			foreach (var plane in new[] {PlaneXY, PlaneYZ, PlaneZX})
			{
				plane.MouseDown += OnMouseDown;
				plane.MouseLeave += OnMouseLeave;
				plane.MouseOver += OnMouseOverPlane;
			}
		}

		private void OnMouseOverPlane(object sender, EventArgs eventArgs)
		{
			ChangeConstraints((sender as Plane).Constraint);
		}

		private void OnMouseOverAxis(object sender, EventArgs eventArgs)
		{
			ChangeConstraints((sender as Axis).Constraint);
		}

		private void ChangeConstraints(Constraints cons)
		{
			if (cons != constraints)
			{
				constraints = cons;
				EventHandler handler = ViewChanged;
				if (handler != null) handler(this, EventArgs.Empty);
			}
		}

		private void OnMouseDown(object sender, EventArgs eventArgs)
		{
			fixedConstraints = constraints;
		}

		private void OnMouseLeave(object sender, EventArgs eventArgs)
		{
			ChangeConstraints(fixedConstraints);
		}

		public IEnumerable<Controller> GetControllers()
		{
			yield return AxisX;
			yield return AxisY;
			yield return AxisZ;
			yield return PlaneXY;
			yield return PlaneYZ;
			yield return PlaneZX;
		}

		public void Draw(Vector3 gizmoPos, Camera camera, Size controlSize)
		{
			float scale = (float) camera.GetPerspectiveRatio(gizmoPos)*GizmoSizePx/controlSize.Height;
			
			AxisX.AdjustPosition(scale, gizmoPos);
			AxisY.AdjustPosition(scale, gizmoPos);
			AxisZ.AdjustPosition(scale, gizmoPos);

			PlaneXY.AdjustPosition(scale, gizmoPos);
			PlaneYZ.AdjustPosition(scale, gizmoPos);
			PlaneZX.AdjustPosition(scale, gizmoPos);

			GL.PushMatrix();
			GL.Translate(gizmoPos);
			GL.Scale(scale, scale, scale);
			
			AxisX.Draw(AxisY, AxisZ, constraints, camera);
			AxisY.Draw(AxisX, AxisZ, constraints, camera);
			AxisZ.Draw(AxisX, AxisY, constraints, camera);
			
			PlaneXY.Draw(constraints);
			PlaneYZ.Draw(constraints);
			PlaneZX.Draw(constraints);

			GL.PopMatrix();
		}
	}
}

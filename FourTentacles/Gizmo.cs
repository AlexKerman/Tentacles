using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Gizmo : Controller
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

			public Gizmo Gizmo;

			public override void Render(RenderContext context)
			{
				GL.Begin(PrimitiveType.Triangles);
				GL.Vertex3(axis1 * QuadSize * Gizmo.scale);
				GL.Vertex3(axis2 * QuadSize * Gizmo.scale);
				GL.Vertex3((axis1 + axis2) * QuadSize * Gizmo.scale);
				GL.End();
			}

			public override Vector3 Pos
			{
				get { return Gizmo.Pos; }
				set { }
			}

			public override void OnMouseDown()
			{
				Gizmo.fixedConstraints = Gizmo.constraints;
			}

			public override void OnMouseOver()
			{
				Gizmo.ChangeConstraints(constraint);
			}

			public override void OnMouseLeave()
			{
				Gizmo.ChangeConstraints(Gizmo.fixedConstraints);
			}

			public override void OnMouseDrag(MouseMoveParams e)
			{
				Gizmo.OnMouseDrag(e);
			}

			public override Cursor GetCursor()
			{
				return EditorCursors.Move;
			}

			public Plane(Vector3 axis1, Vector3 axis2, Constraints constraint)
			{
				this.axis1 = axis1;
				this.axis2 = axis2;
				this.constraint = constraint;
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
		}

		class Axis : Controller
		{
			private const int ArrowSides = 6;
			private const float ArrowSize = 0.3f;
			private const float ArrowWidth = 0.05f;

			private const float SignSize = 0.07f;

			private SinCosTable sinCos = new SinCosTable(ArrowSides);
			public Gizmo Gizmo;
			private readonly Color color;
			private readonly Vector3 axisVector;
			private readonly Constraints constraint;
			private readonly Vector2[] sign;
			private readonly static Color SelectedColor = Color.Yellow;

			public Axis(Color color, Vector3 axisVector, Constraints constraint, Vector2[] sign)
			{
				this.color = color;
				this.axisVector = axisVector;
				this.constraint = constraint;
				this.sign = sign;
			}

			public override void Render(RenderContext context)
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(axisVector * Gizmo.scale * QuadSize);
				GL.Vertex3(axisVector * Gizmo.scale);
				GL.End();
			}

			public override Vector3 Pos
			{
				get { return Gizmo.Pos; }
				set { }
			}

			public override void OnMouseDown()
			{
				Gizmo.fixedConstraints = Gizmo.constraints;
			}

			public override void OnMouseOver()
			{
				Gizmo.ChangeConstraints(constraint);
			}

			public override void OnMouseLeave()
			{
				Gizmo.ChangeConstraints(Gizmo.fixedConstraints);
			}

			public override void OnMouseDrag(MouseMoveParams e)
			{
				Gizmo.OnMouseDrag(e);
			}

			public override Cursor GetCursor()
			{
				return EditorCursors.Move;
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
				foreach (Vector3 ringPoint in sinCos.Points(axis1.axisVector, axis2.axisVector))
					GL.Vertex3((1.0f - ArrowSize) * axisVector + ArrowWidth * ringPoint);
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
				axis.Gizmo = this;
			foreach (var plane in new[] {PlaneXY, PlaneYZ, PlaneZX})
				plane.Gizmo = this;
		}

		public Vector3 ConstrainVector(Vector3 vec)
		{
			Vector3 result = Vector3.Zero;
			if (constraints.HasFlag(Constraints.X)) result.X = vec.X;
			if (constraints.HasFlag(Constraints.Y)) result.Y = vec.Y;
			if (constraints.HasFlag(Constraints.Z)) result.Z = vec.Z;
			return result;
		}

		private void ChangeConstraints(Constraints cons)
		{
			if (cons != constraints)
			{
				constraints = cons;
				ViewChanged.Raise();
			}
		}

		public IEnumerable<Controller> GetControllers()
		{
			yield return AxisX;
			yield return AxisY;
			yield return AxisZ;
			yield return PlaneXY;
			yield return PlaneYZ;
			yield return PlaneZX;
			if (SelectedNodes.Any()) yield return this;
		}

		private float scale;
		public void Draw(Vector3 gizmoPos, Camera camera)
		{
			scale = (float) camera.GetPerspectiveRatio(gizmoPos)*GizmoSizePx;
			Pos = gizmoPos;

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

		public override Cursor GetCursor()
		{
			return EditorCursors.Move;
		}

		public List<Node> SelectedNodes;

		public override void Render(RenderContext context)
		{
			foreach (var node in SelectedNodes)
				node.Render(context);
		}

		public event EventHandler<MouseMoveEventArgs> MoveObjects;

		public override void OnMouseDrag(MouseMoveParams e)
		{
			MoveObjects.Raise(new MouseMoveEventArgs {Vec = e.Constrained});
		}

		public class MouseMoveEventArgs : EventArgs
		{
			public Vector3 Vec;
		}
	}
}

﻿using System;
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

		class Axis
		{
			private const int ArrowSides = 6;
			private const float ArrowSize = 0.3f;
			private const float ArrowWidth = 0.05f;
			public const float QuadSize = 0.3f;
			private const float SignSize = 0.07f;

			private SinCosTable sinCos = new SinCosTable(ArrowSides);
			private readonly Color color;
			private readonly Vector3 axisVector;
			private readonly Constraints constraint;
			private readonly Vector2[] sign;
			private readonly static Color SelectedColor = Color.Yellow;

			public ArrowController ArrowController { get; private set; }

			public Axis(Color color, Vector3 axisVector, Constraints constraint, Vector2[] sign)
			{
				this.color = color;
				this.axisVector = axisVector;
				this.constraint = constraint;
				this.sign = sign;

				ArrowController = new ArrowController(axisVector, constraint);
			}

			public void Draw(Axis axis1, Axis axis2, Constraints constraints, Camera camera)
			{
				Vector3 signAxisX = camera.Right * SignSize;
				Vector3 signAxisY = camera.Top * SignSize;

				GL.Begin(BeginMode.Lines);

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
				GL.Begin(BeginMode.TriangleFan);
				GL.Vertex3(axisVector);
				for (int i = 0; i <= ArrowSides; i++)
				{
					Vector3 ringPoint = sinCos.RingPoint(axis1.axisVector, axis2.axisVector, i);
					GL.Vertex3((1.0f - ArrowSize) * axisVector + ArrowWidth * ringPoint);
				}
				GL.End();
			}
		}

		private const int GizmoSizePx = 64;
		private Constraints fixedConstraints = Constraints.X | Constraints.Y;
		private Constraints constraints = Constraints.X | Constraints.Y;

		private Axis AxisX = new Axis(Color.DarkRed, Vector3.UnitX, Constraints.X, new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0.6f,0), new Vector2(0,1)});
		private Axis AxisY = new Axis(Color.DarkGreen, Vector3.UnitY, Constraints.Y, new[] {new Vector2(0,0), new Vector2(0.6f,1), new Vector2(0,1), new Vector2(0.3f,0.5f)});
		private Axis AxisZ = new Axis(Color.DarkBlue, Vector3.UnitZ, Constraints.Z, new[] {new Vector2(0,1), new Vector2(0.6f,1), new Vector2(0.6f,1), new Vector2(0,0), new Vector2(0,0), new Vector2(0.6f,0)});

		public event EventHandler ViewChanged;

		public Gizmo()
		{
			foreach (var axis in new[] {AxisX, AxisY, AxisZ})
			{
				axis.ArrowController.MouseDown += OnMouseDown;
				axis.ArrowController.MouseLeave += OnMouseLeave;
				axis.ArrowController.MouseOver += OnMouseOver;
			}
		}

		private void OnMouseOver(object sender, EventArgs eventArgs)
		{
			ChangeConstraints((sender as ArrowController).Constraint);
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
			constraints = fixedConstraints;
		}

		private void OnMouseLeave(object sender, EventArgs eventArgs)
		{
			ChangeConstraints(fixedConstraints);
		}

		public IEnumerable<Controller> GetControllers()
		{
			yield return AxisX.ArrowController;
			yield return AxisY.ArrowController;
			yield return AxisZ.ArrowController;
		}

		public void Draw(Vector3 gizmoPos, Camera camera, Size controlSize)
		{
			float scale = (float) camera.GetPerspectiveRatio(gizmoPos)*GizmoSizePx/controlSize.Height;
			AxisX.ArrowController.AdjustPosition(scale, gizmoPos);
			AxisY.ArrowController.AdjustPosition(scale, gizmoPos);
			AxisZ.ArrowController.AdjustPosition(scale, gizmoPos);

			GL.PushMatrix();
			GL.Translate(gizmoPos);
			GL.Scale(scale, scale, scale);
			AxisX.Draw(AxisY, AxisZ, constraints, camera);
			AxisY.Draw(AxisX, AxisZ, constraints, camera);
			AxisZ.Draw(AxisX, AxisY, constraints, camera);
			GL.PopMatrix();
		}

		class ArrowController : Controller
		{
			private readonly Vector3 axis;
			private readonly Constraints constraint;

			public ArrowController(Vector3 axis, Constraints constraint)
			{
				this.axis = axis;
				this.constraint = constraint;
			}

			private Vector3 pos;
			private float scale;

			public Constraints Constraint
			{
				get { return constraint; }
			}

			public void AdjustPosition(float scale, Vector3 pos)
			{
				this.scale = scale;
				this.pos = pos;
			}

			public override void DrawShape()
			{
				GL.LineWidth(3.0f);
				GL.Begin(BeginMode.Lines);
				GL.Vertex3(pos + axis*scale*Axis.QuadSize);
				GL.Vertex3(pos + axis*scale);
				GL.End();
			}
		}
	}
}

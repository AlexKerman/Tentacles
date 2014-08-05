using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	public class Camera
	{
		private RollInterpolator zoom = new RollInterpolator();

		private Vector3 Pos;
		private Vector3 target;
		private Vector3 top;

		private float fieldOfViev;
		private double fovAtan;

		private float FieldOfViev
		{
			get { return fieldOfViev; }
			set
			{
				fieldOfViev = value;
				fovAtan = Math.Atan(fieldOfViev);
			}
		}

		private Size controlSize;

		public Camera()
		{
			FieldOfViev = MathHelper.PiOver4;   //45 degree by default
			target = Vector3.Zero;
			top = Vector3.UnitY;
			Pos = new Vector3(-3600, 1200, 0);
		}

		public Vector3 Top
		{
			get { return top; }
		}

		public Vector3 Right
		{
			get
			{
				Vector3 front = target - Pos;
				Vector3 right = Vector3.Cross(front, top);
				right.Normalize();
				return right;
			}
		}

		Matrix4 modelViewMatrix;
		Matrix4 projectionMatrix;
		

		public void SetProjectionMatrix(Size size) { SetProjectionMatrix(size, Matrix4d.Identity);}
		public void SetProjectionMatrix(Size size, Matrix4d selectionMatrix)
		{
			controlSize = size;
			GL.Viewport(0, 0, size.Width, size.Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref selectionMatrix);
			
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfViev, size.Width / (float)size.Height, 1.0f, 30000.0f);
			GL.MultMatrix(ref projectionMatrix);
			projectionMatrix.Transpose();

			GL.MatrixMode(MatrixMode.Modelview);
			modelViewMatrix = Matrix4.LookAt(Pos, target, top);
			GL.LoadMatrix(ref modelViewMatrix);
		}

		public Vector2 WordToScreen(Vector3 vec)
		{
			var v3 = Vector3.Transform(vec, modelViewMatrix);
			var v4 = Vector4.Transform(new Vector4(v3), projectionMatrix);
			return new Vector2(
				(v4.X / v4.W + 0.5f) * controlSize.Width,
				(0.5f - v4.Y / v4.W) * controlSize.Height);
		}

		public Vector3 VectorToCam(Vector3 absolute)
		{
			return Vector3.Normalize(Pos - absolute);
		}

		public void SetOrtho()
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, controlSize.Width, controlSize.Height, 0, -10000, 10000);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
		}

		/// <summary>
		/// Calculates perspective ratio for object
		/// </summary>
		/// <param name="point">Location of object</param>
		/// <returns>Size in pt of each object pixel</returns>
		public double GetPerspectiveRatio(Vector3 point)
		{
			double distance = (Pos - point).Length;
			return fovAtan*distance/controlSize.Height;
		}

		public bool Moving
		{
			get { return zoom.Active; }
		}

		public void Update()
		{
			if (zoom.Active)
			{
				Pos = zoom.GetCameraPos(Pos, target);
			}
		}

		public void Roll(float scale)
		{
			zoom.Roll(Pos, target, scale);
		}

		public void Move(Vector3 move)
		{
			Vector3 movvy = top;
			Vector3 vec = target - Pos;
			Vector3 movvx = Vector3.Cross(vec, movvy);
			movvx.Normalize();
			movvx *= -move.X;
			movvy *= move.Y;

			Pos += movvx + movvy;
			target += movvx + movvy;
		}

		public void Rotate(float x, float y)
		{
			Vector3 xaxis = Vector3.Cross(target - Pos, top);
			xaxis.Normalize();
			Quaternion roty = Quaternion.FromAxisAngle(xaxis, y);
			Quaternion rotx = Quaternion.FromAxisAngle(Vector3.UnitY, x);
			var rotate = roty * rotx;

			Vector3 vec = Pos - target;
			vec = Vector3.Transform(vec, rotate);
			Pos = vec + target;

			//top = Vector3.Transform(top, rotate);
			AdjustTopVector();
		}

		private void AdjustTopVector()
		{
			Vector3 front = target - Pos;
			front.Normalize();
			Vector3 left = Vector3.Cross(top, front);
			left.Y = 0.0f;
			top = Vector3.Cross(front, left);
			top.Normalize();
		}

		public Vector3 ScreenVectorToWorld(Point delta, Vector3 objectPos)
		{
			double scale = GetPerspectiveRatio(objectPos);
			Vector3 move = Right*delta.X;
			move -= Top*delta.Y;
			move *= (float) scale;
			return move;
		}
	}
}

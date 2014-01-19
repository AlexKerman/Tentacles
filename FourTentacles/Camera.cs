using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Camera
	{
		private RollInterpolator zoom = new RollInterpolator();

		private Vector3 Pos;
		private Vector3 target;
		private Vector3 top;
		private float FieldOfViev { get; set; }

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

		public void SetProjectionMatrix(Size size) { SetProjectionMatrix(size, Matrix4d.Identity);}
		public void SetProjectionMatrix(Size size, Matrix4d selectionMatrix)
		{
			GL.Viewport(0, 0, size.Width, size.Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref selectionMatrix);

			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(FieldOfViev, size.Width / (float)size.Height, 1.0f, 30000.0f);
			GL.MultMatrix(ref projection);

			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4 projectionMatrix = Matrix4.LookAt(Pos, target, top);
			GL.LoadMatrix(ref projectionMatrix);
		}

		public double GetPerspectiveRatio(Vector3 point)
		{
			double distance = (Pos - point).Length;
			return Math.Tan(FieldOfViev)*distance;
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
			Vector3 movvx;		//вектор движения по горизонтали
			Vector3 movvy;		//вектор движения по вертикали
			Vector3 vec;

			movvy = top;
			vec = target - Pos;
			movvx = Vector3.Cross(vec, movvy);
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

			top = Vector3.Transform(top, rotate);
			AdjustTopVector();
		}

		private void AdjustTopVector()
		{
			Vector3 front = target - Pos;
			front.Normalize();
			Vector3 left = Vector3.Cross(top, front);
			top = Vector3.Cross(front, left);
			top.Normalize();
		}
	}
}

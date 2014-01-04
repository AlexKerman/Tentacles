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
	class SelectionRectangle
	{
		private Point startLocation;
		private Size controlSize;
		private Point endLocaton;

		public SelectionRectangle(Point startLocation, Size controlSize)
		{
			this.startLocation = startLocation;
			this.endLocaton = startLocation;
			this.controlSize = controlSize;
		}

		public int Width
		{
			get { return Math.Abs(startLocation.X - endLocaton.X); }
		}

		public int Height
		{
			get { return Math.Abs(startLocation.Y - endLocaton.Y); }
		}

		public bool IsPoint
		{
			get { return startLocation == endLocaton; }
		}

		public Point EndLocaton
		{
			get { return endLocaton; }
			set { endLocaton = value; }
		}

		public Matrix4d GetSelectionMatrix()
		{
			double width = Width;
			double height = Height;
			if (IsPoint) width = height = 2.0;

			return new Matrix4d(
				new Vector4d(controlSize.Width / width, 0, 0, 0),
				new Vector4d(0, controlSize.Height / height, 0, 0),
				new Vector4d(0, 0, 1, 0),
				new Vector4d((controlSize.Width - startLocation.X - endLocaton.X) / width, 
					(startLocation.Y + endLocaton.Y - controlSize.Height) / height, 0, 1));
		}

		public void Draw()
		{
			int y1 = controlSize.Height - startLocation.Y;
			int y2 = controlSize.Height - endLocaton.Y;
			int x1 = startLocation.X;
			int x2 = endLocaton.X;

			//GL.Disable(EnableCap.Lighting);
			//GL.ShadeModel(ShadingModel.Flat);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, controlSize.Width, 0, controlSize.Height, -10000, 10000);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Color4(1, 1, 1, 0.3f);
			GL.Begin(BeginMode.Quads);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y1);
			GL.Vertex2(x2, y2);
			GL.Vertex2(x1, y2);
			GL.End();

			GL.Color4(1, 1, 1, 1.0f);
			GL.Begin(BeginMode.LineLoop);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y1);
			GL.Vertex2(x2, y2);
			GL.Vertex2(x1, y2);
			GL.End();
		}
	}
}

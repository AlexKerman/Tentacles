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

		public bool IsPoint
		{
			get { return startLocation == endLocaton; }
		}

		public Point EndLocaton
		{
			get { return endLocaton; }
			set { endLocaton = value; }
		}

		public void SelectObjects(IEnumerable<Node> selectable, RenderContext context)
		{
			var objects = selectable.ToList();
			selectionBuffer = new uint[4 * objects.Count];
			GL.SelectBuffer(selectionBuffer.Length, selectionBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			context.Camera.SetProjectionMatrix(controlSize, GetSelectionMatrix());
			GL.Translate(context.AbsolutePosition);
			GL.RenderMode(RenderingMode.Select);

			GL.InitNames();
			for (int i = 0; i < objects.Count; i++)
			{
				GL.PushName(i);
				GL.PushMatrix();
				GL.Translate(objects[i].Pos);
				objects[i].Render(context);
				GL.PopMatrix();
				GL.PopName();
			}
			GL.Finish();

			selectedCount = GL.RenderMode(RenderingMode.Render);

			// если просто щелчок, то ищем один самый ближний объект
			if (selectedCount > 0 && IsPoint)
			{
				for (int i = 0; i < selectedCount; i++)
					if (selectionBuffer[4 * i + 1] < selectionBuffer[1])
					{
						selectionBuffer[1] = selectionBuffer[4 * i + 1];
						selectionBuffer[3] = selectionBuffer[4 * i + 3];
					}
				selectedCount = 1;
			}
		}

		private int selectedCount;
		private uint[] selectionBuffer;

		public int SelectedCount
		{
			get { return selectedCount; }
		}

		public IEnumerable<int> SelectedIndicies
		{
			get
			{
				for (int i = 0; i < selectedCount; i++)
					yield return (int)selectionBuffer[4 * i + 3];
			}
		}

		private Matrix4d GetSelectionMatrix()
		{
			double width = Math.Abs(startLocation.X - endLocaton.X);
			double height = Math.Abs(startLocation.Y - endLocaton.Y);
			if (IsPoint) width = height = 5.0;

			return new Matrix4d(
				new Vector4d(controlSize.Width / width, 0, 0, 0),
				new Vector4d(0, controlSize.Height / height, 0, 0),
				new Vector4d(0, 0, 1, 0),
				new Vector4d((controlSize.Width - startLocation.X - endLocaton.X) / width, 
					(startLocation.Y + endLocaton.Y - controlSize.Height) / height, 0, 1));
		}

		public void Draw(RenderContext context)
		{
			int y1 = controlSize.Height - startLocation.Y;
			int y2 = controlSize.Height - endLocaton.Y;
			int x1 = startLocation.X;
			int x2 = endLocaton.X;

			context.Camera.SetOrtho();
			GL.Disable(EnableCap.LineSmooth);
			GL.Disable(EnableCap.DepthTest);

			GL.Color4(1, 1, 1, 0.3f);
			GL.Begin(PrimitiveType.Quads);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y1);
			GL.Vertex2(x2, y2);
			GL.Vertex2(x1, y2);
			GL.End();

			GL.Color4(1, 1, 1, 1.0f);
			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex2(x1, y1 + 1);
			GL.Vertex2(x2, y1);
			GL.Vertex2(x2, y2);
			GL.Vertex2(x1, y2);
			GL.End();
		}
	}
}

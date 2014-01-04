using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	[Flags]
	public enum RenderMode
	{
		Solid = 1,
		Wireframe = 2,
		SolidWireframe = 3,
		Normals = 4
	}

	public enum SelectMode
	{
		New, Add, Sub
	}

	public partial class MainWindow : Form
	{
		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState(Int32 vKey);
		private const int VK_MENU = 0x12;	//Any Alt key

		private Camera camera = new Camera();
		private List<Spline4D> splines = new List<Spline4D>();
		private List<Spline4D> selectedSplines = new List<Spline4D>();

		private SelectionRectangle selectionRectangle = null;
		private Gizmo gizmo = new Gizmo();

		private bool refresh = true;

		public MainWindow()
		{
			InitializeComponent();
			glc.MouseWheel += OnMouseWheel;

			var spline = new Spline4D(48, 96);
			spline.AddSegment(
				new Vector4(0.0f, 200.0f, 0.0f, 50.0f),
				new Vector4(0.0f, 1200.0f, -200.0f, 0.0f), 
				new Vector4(0.0f, 800.0f, 0.0f, -150.0f),
				new Vector4(0.0f, 600.0f, 800.0f, 200.0f));
			splines.Add(spline);

			Process prc = new Process();
			prc.StartInfo.FileName = @"http://site.su";
			prc.StartInfo.UseShellExecute = true;
			prc.Start();
		}

		private void OnShown(object sender, EventArgs e)
		{
			InitRender();
			Render();
			UpdateSelectionModeLabel();
		}

		private void InitRender()
		{
			glc.MakeCurrent();

			GL.ClearDepth(1.0f);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Light0);
			
			//GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); // Really Nice Perspective Calculations

			// Light model parameters:
			// -------------------------------------------
			GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 0.0f, 0.0f, 0.0f, 0.0f, });
			GL.LightModel(LightModelParameter.LightModelLocalViewer, 1.0f);
			GL.LightModel(LightModelParameter.LightModelTwoSide, 0.0f);
		}

		int oldx, oldy;     //previous event mouse coordinates
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (selectionRectangle != null)
			{
				selectionRectangle.EndLocaton = e.Location;
				Render();
			}

			if ((e.Button & MouseButtons.Middle) != 0)
			{
				if (GetAsyncKeyState(VK_MENU) != 0) camera.Rotate((oldx - e.X) / 100.0f, (oldy - e.Y) / 100.0f);
				else camera.Move(new Vector3(e.X - oldx, e.Y - oldy, 0));
				Render();
			}
			oldx = e.X;
			oldy = e.Y;
		}

		private void OnMouseWheel(object sender, MouseEventArgs e)
		{
			camera.Roll(e.Delta > 0 ? 0.3f : -0.3f);
			Render();
		}

		private void OnMouseButtonPressed(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) != 0)
			{
				selectionRectangle = new SelectionRectangle(e.Location, glc.Size);
			}

			oldx = e.X;
			oldy = e.Y;
		}

		private void OnMouseButtonReleased(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && selectionRectangle != null)
			{
				SelectObjects();
				selectionRectangle = null;
				Render();
			}
		}

		private void SelectObjects()
		{
			selectedSplines.Clear();
			
			uint[] selectionBuffer = new uint[4 * splines.Count];
			GL.SelectBuffer(selectionBuffer.Length, selectionBuffer);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			camera.SetProjectionMatrix(glc.Size, selectionRectangle.GetSelectionMatrix());
			GL.RenderMode(RenderingMode.Select);

			GL.InitNames();
			for (int i = 0; i < splines.Count; i++)
			{
				GL.PushName(i);
				splines[i].Render(RenderMode.Solid);
				GL.PopName();
			}
			GL.Finish();

			int selectedCount = GL.RenderMode(RenderingMode.Render);

			// если просто щелчок, то ищем один самый ближний объект
			if (selectedCount > 0 && selectionRectangle.IsPoint)
			{
				for (int i = 0; i < selectedCount; i++)
					if (selectionBuffer[4 * i + 1] < selectionBuffer[1])
					{
						selectionBuffer[1] = selectionBuffer[4 * i + 1];
						selectionBuffer[3] = selectionBuffer[4 * i + 3];
					}
				selectedCount = 1;
			}

			for (int i = 0; i < selectedCount; i++)
			{
				int uname = (int)selectionBuffer[4 * i + 3];
				var selected = splines[uname];
				if(!selectedSplines.Contains(selected))
					selectedSplines.Add(selected);
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			Render();
		}
		
		private bool rendering = false;
		private bool sceneUpdated = false;
		private void Render()
		{
			sceneUpdated = true;
			if (rendering) return;

			rendering = true;
			while (camera.Moving || sceneUpdated)
			{
				Application.DoEvents();
				sceneUpdated = false;
				camera.Update();
				RenderFrame();
			}
			rendering = false;
		}

		RenderMode renderMode = RenderMode.Solid;
		private void RenderFrame()
		{
			GL.ClearColor(Color.LightBlue);
			GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
			GL.Enable(EnableCap.DepthTest);

			Material.Init();
			camera.SetProjectionMatrix(glc.Size);

			DrawGrid();

			foreach (var spline in splines)
				spline.Render(renderMode);

			var box = new BoundingBox();
			foreach (var spline in selectedSplines)
			{
				var bb = spline.GetBoundingBox();
				box = box.Extend(bb);
				bb.Draw();
			}

			GL.Clear(ClearBufferMask.DepthBufferBit);

			if (box.Initialized)
			{
				gizmo.Draw(box.Center, camera, glc.Size);
			}

			if (selectionRectangle != null) selectionRectangle.Draw();
			glc.SwapBuffers();

			lbTrianglesCount.Text = splines.Sum(s => s.GetTrianglesCount()).ToString();
		}

		void DrawGrid()
		{
			Material.SetLineMaterial(Color.DarkGray);
			float i;
			GL.Begin(BeginMode.Lines);
			GL.Color3(.5, .5, .5);
			for (i = -3000; i <= 3000; i += 300)
			{
				GL.Vertex3(-3000, 0, i);
				GL.Vertex3(3000, 0, i);
				GL.Vertex3(i, 0, -3000);
				GL.Vertex3(i, 0, 3000);
			}
			GL.End();
		}

		private void glc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.F3)
			{
				renderMode++;
				if ((renderMode & RenderMode.SolidWireframe) == 0)
					renderMode -= 3;
				Render();
				UpdateSelectionModeLabel();
			}
			if (e.KeyCode == Keys.F4)
			{
				renderMode ^= RenderMode.Normals;
				Render();
				UpdateSelectionModeLabel();
			}
		}

		private void UpdateSelectionModeLabel()
		{
			lbRenderMode.Text = renderMode.ToString();
		}


	}
}

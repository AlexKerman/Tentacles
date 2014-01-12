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
using OpenCLNet;
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
		private Controller mouseOverController;

		private SelectionRectangle selectionRectangle = null;
		private Gizmo gizmo = new Gizmo();

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

			gizmo.ViewChanged += (o, args) => Render();
			gizmo.MoveObjects += GizmoOnMoveObjects;

			glc.Cursor = Cursors.Select;

			//switch Optimus graphics card to NVIDIA
			//https://github.com/opentk/opentk/issues/46
			var openCLPlatform = OpenCL.GetPlatform(0);
		}

		private void GizmoOnMoveObjects(object sender, Vector3 delta)
		{
			foreach (var spline in selectedSplines)
			{
				spline.Pos += delta;
			}
			Render();
		}

		private void OnShown(object sender, EventArgs e)
		{
			InitRender();
			Render();
			UpdateSelectionModeLabel();

			string renderer = GL.GetString(StringName.Renderer);
			string version = GL.GetString(StringName.Version);
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
			int deltaX = e.X - oldx;
			int deltaY = e.Y - oldy;
			oldx = e.X;
			oldy = e.Y;

			if (selectionRectangle != null)
			{
				selectionRectangle.EndLocaton = e.Location;
				Render();
				return;
			}

			if ((e.Button & MouseButtons.Middle) != 0)
			{
				if (GetAsyncKeyState(VK_MENU) != 0) camera.Rotate((-deltaX) / 100.0f, (-deltaY) / 100.0f);
				else camera.Move(new Vector3(deltaX, deltaY, 0));
				Render();
				return;
			}

			if (mouseOverController != null && e.Button == MouseButtons.Left)
			{
				double scale = camera.GetPerspectiveRatio(mouseOverController.Pos) / glc.Height;
				Vector3 move = camera.Right * deltaX;
				move -= camera.Top * deltaY;
				move *= (float) scale;
				mouseOverController.OnMouseDrag(move);
				return;
			}

			if (selectedSplines.Count > 0)
			{
				Controller controller = GetControllerUnderCursor(e.Location);
				if (mouseOverController != null && controller != mouseOverController)
				{
					mouseOverController.OnMouseLeave();
					mouseOverController = null;
					glc.Cursor = Cursors.Select;
					return;
				}
				if (controller != null)
				{
					mouseOverController = controller;
					controller.OnMouseOver();
					glc.Cursor = gizmo.GetCursor();
				}
			}
		}

		private Controller GetControllerUnderCursor(Point mousePosition)
		{
			var rect = new SelectionRectangle(mousePosition, glc.Size);
			var controllers = gizmo.GetControllers().ToList();
			rect.SelectObjects(controllers, camera);
			if (rect.SelectedCount == 0) return null;
			return controllers[rect.SelectedIndicies.First()];
		}

		private void OnMouseWheel(object sender, MouseEventArgs e)
		{
			camera.Roll(e.Delta > 0 ? -0.3f : 0.3f);
			Render();
		}

		private void OnMouseButtonPressed(object sender, MouseEventArgs e)
		{
			oldx = e.X;
			oldy = e.Y;

			if (e.Button == MouseButtons.Left)
			{
				if (mouseOverController != null)
				{
					mouseOverController.OnMouseDown();
					return;
				}

				selectionRectangle = new SelectionRectangle(e.Location, glc.Size);
			}
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
			selectionRectangle.SelectObjects(splines, camera);
			foreach (int i in selectionRectangle.SelectedIndicies)
				selectedSplines.Add(splines[i]);
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			Render();
		}
		
		private bool rendering = false;

		private void Render()
		{
			if (rendering) return;
			rendering = true;
			do
			{
				Application.DoEvents();
				camera.Update();
				RenderFrame();
			} while (camera.Moving);

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
			{
				GL.PushMatrix();
				GL.Translate(spline.Pos);
				spline.Render(renderMode);
				GL.PopMatrix();
			}

			var box = new BoundingBox();
			foreach (var spline in selectedSplines)
				box = box.Extend(spline.GetBoundingBox());
			box.Draw();

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
			GL.Begin(PrimitiveType.Lines);
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

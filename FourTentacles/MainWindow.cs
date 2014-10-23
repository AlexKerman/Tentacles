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
using FourTentacles.Map;
using OpenCLNet;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	[Flags]
	public enum RenderMode
	{
		Selection = 0,
		Solid = 1,
		Wireframe = 2,
		SolidWireframe = 3,
		Normals = 4,
	}

	public partial class MainWindow : Form
	{
		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState(Int32 vKey);
		private const int VK_MENU = 0x12;	//Any Alt key

		private Camera camera = new Camera();
		private Controller mouseOverController;

		private SelectionRectangle selectionRectangle = null;
		private Gizmo gizmo = new Gizmo();

		private SceneNode sceneNode = new SceneNode();

		public MainWindow()
		{
			InitializeComponent();
			glc.MouseWheel += OnMouseWheel;

			sceneNode.RedrawRequired += OnRenderRequested;
			UndoStack.StackChanged += UndoStackOnStackChanged;

			//switch Optimus graphics card to NVIDIA
			//https://github.com/opentk/opentk/issues/46
			//var openCLPlatform = OpenCL.GetPlatform(0);
		}

		private void UndoStackOnStackChanged(object sender, EventArgs eventArgs)
		{
			//todo: update undo/redo buttons
		}

		private void OnRenderRequested(object sender, EventArgs eventArgs)
		{
			Render();
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

			// Light model parameters:
			// -------------------------------------------
			GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 0.0f, 0.0f, 0.0f, 0.0f });
			GL.LightModel(LightModelParameter.LightModelLocalViewer, 1.0f);
			GL.LightModel(LightModelParameter.LightModelTwoSide, 0.0f);
		}

		#region mouse events

		private Point mouseLocation;

		private void OnDoubleClick(object sender, EventArgs e)
		{
			if (mouseOverController != null)
			{
				var mouseOverParams = new MouseOverParams(mouseLocation);
				mouseOverController.OnMouseDoubleClick(mouseOverParams);
				if(mouseOverParams.Changed) Render();
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			Point delta = Point.Subtract(e.Location, new Size(mouseLocation));
			mouseLocation = e.Location;

			if (selectionRectangle != null)
			{
				selectionRectangle.EndLocaton = e.Location;
				Render();
				return;
			}

			if (e.Button.HasFlag(MouseButtons.Middle))
			{
				if (GetAsyncKeyState(VK_MENU) != 0) camera.Rotate((-delta.X) / 100.0f, (-delta.Y) / 100.0f);
				else camera.Move(new Vector3(delta.X, delta.Y, 0));
				Render();
				return;
			}

			if (mouseOverController != null && e.Button == MouseButtons.Left)
			{
				var move = camera.ScreenVectorToWorld(delta, mouseOverController.Pos);
				var constrainedMove = gizmo.ConstrainVector(move);
				mouseOverController.OnMouseDrag(new MouseMoveParams(e.Location, delta, move, constrainedMove));
				Render();
				return;
			}

			var mouseOverParams = new MouseOverParams(e.Location);
			var controller = GetControllerUnderCursor(e.Location);
			if (mouseOverController != null && controller != mouseOverController)
			{
				mouseOverController.OnMouseLeave(mouseOverParams);
				mouseOverController = null;
			}
			if (controller != null)
			{
				mouseOverController = controller;
				controller.OnMouseOver(mouseOverParams);
			}
		
			glc.Cursor = mouseOverParams.Cursor;
			if(mouseOverParams.Changed)
				Render();
		}

		private Controller GetControllerUnderCursor(Point mousePosition)
		{
			var rect = new SelectionRectangle(mousePosition, glc.Size);

			gizmo.SelectedNodes = sceneNode.GetNodes().Where(n => n.IsSelected).ToList();
			var controllers = gizmo.GetControllers().ToList();

			controllers.Add(new SelectNodeController(sceneNode.GetNodes().Where(n => !n.IsSelected)));
			controllers.AddRange(sceneNode.GetControllers());

			rect.SelectObjects(controllers, new RenderContext(camera, sceneNode.GetNodesPos(), RenderMode.Selection));
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
			mouseLocation = e.Location;

			if (e.Button != MouseButtons.Left) return;

			if (mouseOverController != null)
			{
				mouseOverController.OnMouseDown(new MouseOverParams(e.Location));

				//if controller handeled mouse down
				if (!(mouseOverController is SelectNodeController))
					return;
			}

			selectionRectangle = new SelectionRectangle(e.Location, glc.Size);
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

		#endregion mouse events

		private void SelectObjects()
		{
			var nodes = sceneNode.GetNodes().ToList();
			foreach (var node in nodes)
				node.IsSelected = false;
			selectionRectangle.SelectObjects(nodes, new RenderContext(camera, sceneNode.GetNodesPos(), RenderMode.Selection));
			foreach (int i in selectionRectangle.SelectedIndicies)
				nodes[i].IsSelected = true;
			gizmo.SelectedNodes = nodes.Where(n => n.IsSelected).ToList();

			Control control = sceneNode.GetNodeControl();
			panel1.Controls.Clear();
			if(control != null)
				panel1.Controls.Add(control);

			nodeActionsToolStrip.SetActions(gizmo.SelectedNodes);
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

			var renderContext = new RenderContext(camera, sceneNode.GetNodesPos(), renderMode);
			sceneNode.Render(renderContext);

			var box = sceneNode.GetBoundingBox();

			GL.Clear(ClearBufferMask.DepthBufferBit);
			
			GL.Translate(renderContext.AbsolutePosition);
			if (sceneNode.HasSelectedNodes())
			{
				Vector3 gizmoCenter = Vector3.Zero;
				foreach (var node in sceneNode.GetNodes().Where(n => n.IsSelected))
					gizmoCenter += node.Pos;
				gizmoCenter /= sceneNode.GetNodes().Count(n => n.IsSelected);

				gizmo.Draw(gizmoCenter, camera);
				box.Draw();
			}

			if (selectionRectangle != null) selectionRectangle.Draw(renderContext);
			glc.SwapBuffers();

			lbTrianglesCount.Text = sceneNode.GetTrianglesCount().ToString();
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

		private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.Z)
			{
				if (e.Shift) UndoStack.Redo();
				else UndoStack.Undo();
				Render();
			}

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

		private void fromOSMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var importForm = new ImportOsmFile();
			if (importForm.ShowDialog() == DialogResult.OK)
			{
				sceneNode.AddGeometry(importForm.Map);
				Render();
			}
		}
	}
}

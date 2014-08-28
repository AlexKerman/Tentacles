using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace FourTentacles
{
	class NodeActionsToolstrip : ToolStrip
	{
		public event EventHandler RenderRequested;

		private ToolStripButton btNodeCusp = new ToolStripButton("Make nodes cusp", Icons.NodeCusp);
		private ToolStripButton btNodeSmooth = new ToolStripButton("Make nodes smooth", Icons.NodeSmooth);
		private ToolStripButton btNodeSymmetrical = new ToolStripButton("Make nodes symmetrical", Icons.NodeSymmetrical);

		private void OnNodeSymmetricalClicked(object sender, EventArgs e)
		{
			changePointSmoothMode.ChangeMode(PointSmoothMode.Symmetrical);
			SetButtonsChecked();
			RenderRequested.Raise();
		}

		private void OnNodeSmoothClicked(object sender, EventArgs e)
		{
			changePointSmoothMode.ChangeMode(PointSmoothMode.Smooth);
			SetButtonsChecked();
			RenderRequested.Raise();
		}

		private void OnNodeCuspClicked(object sender, EventArgs eventArgs)
		{
			changePointSmoothMode.ChangeMode(PointSmoothMode.Cusp);
			SetButtonsChecked();
			RenderRequested.Raise();
		}

		public NodeActionsToolstrip()
		{
			btNodeCusp.Click += OnNodeCuspClicked;
			btNodeSmooth.Click += OnNodeSmoothClicked;
			btNodeSymmetrical.Click += OnNodeSymmetricalClicked;
			foreach (var button in new[] {btNodeCusp, btNodeSmooth, btNodeSymmetrical})
			{
				button.DisplayStyle = ToolStripItemDisplayStyle.Image;
				button.Enabled = false;
				Items.Add(button);
			}
			Items.Add(new ToolStripSeparator());
		}

		private DoUndoChangePointSmoothMode changePointSmoothMode;

		public void SetActions(IEnumerable<Node> selectedNodes)
		{
			points = selectedNodes.OfType<Point4D>().ToList();
			btNodeCusp.Enabled = btNodeSmooth.Enabled = btNodeSymmetrical.Enabled = points.Count > 0;
			if (points.Count > 0)
			{
				SetButtonsChecked();
				changePointSmoothMode = new DoUndoChangePointSmoothMode(points);
			}
		}

		private List<Point4D> points;

		private void SetButtonsChecked()
		{
			btNodeCusp.Checked = points.All(p => p.SmoothMode == PointSmoothMode.Cusp);
			btNodeSmooth.Checked = points.All(p => p.SmoothMode == PointSmoothMode.Smooth);
			btNodeSymmetrical.Checked = points.All(p => p.SmoothMode == PointSmoothMode.Symmetrical);
		}

		class DoUndoChangePointSmoothMode : IDoUndo
		{
			struct GuideBackup
			{
				private readonly Guide4D guide;
				private readonly Vector4 point;
				private readonly Windrose windrose;

				public GuideBackup(Guide4D guide)
				{
					point = guide.Point;
					windrose = guide.WindRose;
					this.guide = guide;
				}

				public void Restore()
				{
					guide.Point = point;
					guide.WindRose = windrose;
					guide.Changed = true;
				}
			}

			private PointSmoothMode mode;
			private readonly Dictionary<Point4D, PointSmoothMode> points;
			private readonly List<GuideBackup> guides = new List<GuideBackup>();

			public DoUndoChangePointSmoothMode(IEnumerable<Point4D> points)
			{
				this.points = points.ToDictionary(p => p, p => p.SmoothMode);
			}

			public void ChangeMode(PointSmoothMode mode)
			{
				this.mode = mode;
				foreach (var point in points.Keys)
					foreach (var guide in point.Guides)
						guides.Add(new GuideBackup(guide));
				UndoStack.AddAction(this);
				Redo();
			}

			public void Undo()
			{
				foreach (var point in points)
				{
					point.Key.SmoothMode = point.Value;
					point.Key.Changed = true;
				}
				foreach (var guideBackup in guides)
					guideBackup.Restore();
			}

			public void Redo()
			{
				foreach (var point in points.Keys)
				{
					point.SmoothMode = mode;
					if(point.Guides.Any())
						point.Guides[0].AjustSymmetricGuide();
					point.Changed = true;
				}
			}
		}
	}
}

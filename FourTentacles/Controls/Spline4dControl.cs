using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourTentacles
{
	partial class Spline4dControl : GeometryControl
	{
		private readonly Spline4D spline4D;
		private List<Point4D> selectedPoints;

		public Spline4dControl()
		{
			InitializeComponent();
		}

		public Spline4dControl(Spline4D spline4D) : base(spline4D)
		{
			InitializeComponent();
			this.spline4D = spline4D;

			numLengthSubdivide.Value = spline4D.LengthSides;
			numRoundSubdivide.Value = spline4D.RoundSides;
			cbLengthSmooth.Checked = spline4D.LengthSmooth;

			selectedPoints = spline4D.GetNodes().OfType<Point4D>().Where(p => p.IsSelected).ToList();
			if (selectedPoints.Count > 0 && spline4D.SelectionMode == Spline4D.SelectionModeEnum.Points)
				SetJointType();
		}

		private void SetJointType()
		{
			rbPointCusp.Checked = selectedPoints.All(p => p.SmoothMode == PointSmoothMode.Cusp);
			rbPointSymmetric.Checked = selectedPoints.All(p => p.SmoothMode == PointSmoothMode.Symmetric);
			rbPointSmooth.Checked = selectedPoints.All(p => p.SmoothMode == PointSmoothMode.Smooth);
		}

		private void EditPointsClick(object sender, EventArgs e)
		{
			cbEditSegments.Checked = false;
			if (cbEditPoints.Checked)
			{
				spline4D.SelectionMode = Spline4D.SelectionModeEnum.Points;
				OnGeometryLocked();
			}
			else
			{
				spline4D.SelectionMode = Spline4D.SelectionModeEnum.None;
				OnGeometryUnlocked();
			}
		}

		private void EditSegmentsClick(object sender, EventArgs e)
		{
			cbEditPoints.Checked = false;
			if (cbEditSegments.Checked)
			{
				spline4D.SelectionMode = Spline4D.SelectionModeEnum.Segments;
				OnGeometryLocked();
			}
			else
			{
				spline4D.SelectionMode = Spline4D.SelectionModeEnum.None;
				OnGeometryUnlocked();
			}
		}

		private void LengthSubdivideValueChanged(object sender, EventArgs e)
		{
			spline4D.LengthSides = (int) numLengthSubdivide.Value;
			OnRedrawRequired();
		}

		private void RoundSubdivideValueChanged(object sender, EventArgs e)
		{
			spline4D.RoundSides = (int) numRoundSubdivide.Value;
			OnRedrawRequired();
		}

		private void cbLengthSmooth_Click(object sender, EventArgs e)
		{
			spline4D.LengthSmooth = cbLengthSmooth.Checked;
			OnRedrawRequired();
		}
	}
}

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

		public Spline4dControl()
		{
			InitializeComponent();
		}

		public Spline4dControl(Spline4D spline4D) : base(spline4D)
		{
			InitializeComponent();
			this.spline4D = spline4D;

			numLenghtSubdivide.Value = spline4D.LenghtSides;
			numRoundSubdivide.Value = spline4D.RoundSides;
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

		private void LenghtSubdivideValueChanged(object sender, EventArgs e)
		{
			spline4D.LenghtSides = (int) numLenghtSubdivide.Value;
			OnRedrawRequired();
		}

		private void RoundSubdivideValueChanged(object sender, EventArgs e)
		{
			spline4D.RoundSides = (int) numRoundSubdivide.Value;
			OnRedrawRequired();
		}
	}
}

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
			this.spline4D = spline4D;
			InitializeComponent();
		}

		private void cbEditPoints_Click(object sender, EventArgs e)
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

		private void cbEditSegments_Click(object sender, EventArgs e)
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
	}
}

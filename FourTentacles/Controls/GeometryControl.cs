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
	public partial class GeometryControl : UserControl
	{
		public class GeometryLockedEventArgs : EventArgs
		{
			public Geometry geometry;

			public GeometryLockedEventArgs(Geometry geometry)
			{
				this.geometry = geometry;
			}
		}

		protected Geometry geometry;

		#region Events
		public event EventHandler<GeometryLockedEventArgs> GeometryLocked;

		protected virtual void OnGeometryLocked()
		{
			EventHandler<GeometryLockedEventArgs> handler = GeometryLocked;
			if (handler != null) handler(this, new GeometryLockedEventArgs(geometry));
		}

		public event EventHandler<GeometryLockedEventArgs> GeometryUnlocked;

		protected virtual void OnGeometryUnlocked()
		{
			EventHandler<GeometryLockedEventArgs> handler = GeometryUnlocked;
			if (handler != null) handler(this, new GeometryLockedEventArgs(geometry));
		}

		public event EventHandler RedrawRequired;

		protected virtual void OnRedrawRequired()
		{
			EventHandler handler = RedrawRequired;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		public GeometryControl()
		{
			InitializeComponent();
		}

		protected GeometryControl(Geometry geometry)
		{
			this.geometry = geometry;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	abstract class Controller : Node, ISelectable
	{
		public event EventHandler MouseOver;
		public void OnMouseOver()
		{
			EventHandler handler = MouseOver;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler MouseLeave;
		public void OnMouseLeave()
		{
			EventHandler handler = MouseLeave;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler MouseDown;
		public void OnMouseDown()
		{
			EventHandler handler = MouseDown;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler<Vector3> MouseDrag;
		public void OnMouseDrag(Vector3 e)
		{
			EventHandler<Vector3> handler = MouseDrag;
			if (handler != null) handler(this, e);
		}

		public abstract void DrawShape();
	}
}

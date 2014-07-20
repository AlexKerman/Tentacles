using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	public abstract class Controller : Node
	{
		public virtual void OnMouseOver() { }
		public virtual void OnMouseLeave() { }
		public virtual void OnMouseDown() { }
		public virtual void OnMouseDrag(Vector3 e) { }
	}
}

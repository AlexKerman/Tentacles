using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	public class MouseMoveParams
	{
		public readonly Point Delta;
		public readonly Vector3 Move;
		public readonly Vector3 Constrained;
		public readonly Point ScreenPoint;

		public MouseMoveParams(Point screenPoint, Point delta, Vector3 move, Vector3 constrained)
		{
			Delta = delta;
			Move = move;
			Constrained = constrained;
			ScreenPoint = screenPoint;
		}
	}

	public abstract class Controller : Node
	{
		public event EventHandler Changed;

		protected virtual void OnChanged()
		{
			EventHandler handler = Changed;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public virtual void OnMouseOver() { }
		public virtual void OnMouseLeave() { }
		public virtual void OnMouseDown() { }
		public virtual void OnMouseDrag(MouseMoveParams e) { }

		public virtual Cursor GetCursor()
		{
			return Cursors.Default;
		}
	}

	public class SelectNodeController : Controller
	{
		private readonly List<Node> nodes;

		public SelectNodeController(IEnumerable<Node> nodes)
		{
			this.nodes = nodes.ToList();
		}

		public override void Render(RenderContext context)
		{
			foreach (var node in nodes)
			{
				GL.PushMatrix();
				GL.Translate(node.Pos);
				node.Render(context);
				GL.PopMatrix();
			}
		}

		public override Cursor GetCursor()
		{
			return EditorCursors.Select;
		}
	}
}

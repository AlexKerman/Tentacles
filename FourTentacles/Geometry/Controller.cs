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
		public readonly Point Location;
		public readonly Point Delta;
		public readonly Vector3 Move;
		public readonly Vector3 Constrained;
		public readonly Point ScreenPoint;

		public MouseMoveParams(Point screenPoint, Point delta, Vector3 move, Vector3 constrained)
		{
			Location = screenPoint;
			Delta = delta;
			Move = move;
			Constrained = constrained;
			ScreenPoint = screenPoint;
		}
	}

	public class MouseOverParams
	{
		public readonly Point Location;
		public Cursor Cursor = Cursors.Default;
		private bool changed;

		public bool Changed
		{
			get { return changed; }
			set { changed |= value; }
		}

		public MouseOverParams(Point location)
		{
			Location = location;
		}
	}

	public abstract class Controller : Node
	{
		public virtual void OnMouseOver(MouseOverParams mouseOverParams) { }
		public virtual void OnMouseLeave(MouseOverParams mouseOverParams) { }
		public virtual void OnMouseDown(MouseOverParams mouseOverParams) { }
		public virtual void OnMouseDrag(MouseMoveParams e) { }
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

		public override void OnMouseOver(MouseOverParams mouseOverParams)
		{
			mouseOverParams.Cursor = EditorCursors.Select;
		}
	}
}

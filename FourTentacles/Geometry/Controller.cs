using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace FourTentacles
{
	public abstract class Controller : Node
	{
		public virtual void OnMouseOver() { }
		public virtual void OnMouseLeave() { }
		public virtual void OnMouseDown() { }
		public virtual void OnMouseDrag(Vector3 e) { }

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
				node.Render(context);
		}

		public override Cursor GetCursor()
		{
			return EditorCursors.Select;
		}
	}
}

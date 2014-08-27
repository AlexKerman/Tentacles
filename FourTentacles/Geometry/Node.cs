using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourTentacles.Annotations;
using OpenTK;

namespace FourTentacles
{
	public abstract class Node
	{
		/// <summary>
		/// Draw contour for selection
		/// </summary>
		public abstract void Render(RenderContext context);

		public virtual Vector3 Pos { get; set; }
		public virtual bool IsSelected { get; set; }

		public virtual IEnumerable<NodeAction> GetActions(IEnumerable<Node> selectedNodes)
		{
			yield break;
		}
		
		[CanBeNull]
		public virtual GeometryControl GetNodeControl()
		{
			return null;
		}
	}
}

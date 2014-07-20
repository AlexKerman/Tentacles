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
		public abstract void DrawContour(Camera camera, Vector3 basePos);

		public virtual Vector3 Pos { get; set; }
		public virtual bool IsSelected { get; set; }

		[CanBeNull]
		public virtual GeometryControl GetNodeControl()
		{
			return null;
		}
	}
}

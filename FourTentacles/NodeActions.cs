using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FourTentacles
{
	public class NodeActionsGroup
	{
	}

	public abstract class NodeAction
	{
		public abstract Image Icon { get; }
		public abstract string ActionName { get; }
	}
}

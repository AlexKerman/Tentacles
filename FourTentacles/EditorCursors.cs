using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FourTentacles
{
	static class EditorCursors
	{
		public static readonly Cursor Select = new Cursor(new MemoryStream(Properties.Resources.Select));
		public static readonly Cursor Move = new Cursor(new MemoryStream(Properties.Resources.Move));
		public static readonly Cursor AddPoint = new Cursor(new MemoryStream(Properties.Resources.AddPoint));
	}
}

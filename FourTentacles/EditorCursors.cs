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
		public static Cursor Select = new Cursor(new MemoryStream(Properties.Resources.Select));
		public static Cursor Move = new Cursor(new MemoryStream(Properties.Resources.Move));
	}
}

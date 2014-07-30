using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourTentacles
{
	public static class EventsExtensions
	{
		public static void Raise<T>(this EventHandler<T> evt, T e) where T: EventArgs
		{
			if (evt != null)
			{
				evt(null, e);
			}
		}

		public static void Raise(this EventHandler evt)
		{
			if (evt != null)
			{
				evt(null, EventArgs.Empty);
			}
		}
	}
}

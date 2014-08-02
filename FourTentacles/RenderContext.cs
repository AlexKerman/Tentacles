using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	public class RenderContext
	{
		public readonly RenderMode Mode;
		public readonly Camera Camera;
		public Vector3 AbsolutePosition;

		public RenderContext(Camera camera, Vector3 pos, RenderMode renderMode)
		{
			Camera = camera;
			AbsolutePosition = pos;
			Mode = renderMode;
		}

		public Vector2 WorldToScreen(Vector3 vec)
		{
			return Camera.WordToScreen(vec + AbsolutePosition);
		}
	}
}

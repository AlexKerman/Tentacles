using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FourTentacles
{
	class Guide4D
	{
		public readonly GuideWidthController WidthController;
		public readonly Guide4DController GuideController;

		public Vector4 Point;
		public Point4D BasePoint;

		public Guide4D(Point4D basePoint, Vector4 point)
		{
			BasePoint = basePoint;
			Point = point;
			WidthController = new GuideWidthController(this);
			GuideController = new Guide4DController(this);
		}
	}
}

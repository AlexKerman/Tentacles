using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FourTentacles
{
	class Guide4D
	{
		public Point4D BasePoint;
		public readonly GuideWidthController WidthController;
		public readonly Guide4DController GuideController;
		private Vector4 point;
		
		public Vector4 Point
		{
			get { return point; }
			set
			{
				if(point == value) return;
				point = value;
				AjustSymmetricGuide();

				//todo: make for guide Changed prop
				BasePoint.Changed = true;
			}
		}

		private void AjustSymmetricGuide()
		{
			if(BasePoint.SmoothMode == PointSmoothMode.Cusp) return;
			foreach (var guide in BasePoint.Guides)
				if (guide != this)
				{
					//todo: guide Changed prop On
					if (BasePoint.SmoothMode == PointSmoothMode.Symmetrical) guide.point = -point;
					if (BasePoint.SmoothMode == PointSmoothMode.Smooth)
					{
						guide.point.Xyz = Vector3.Normalize(-point.Xyz)*guide.point.Length;
					}
				}
		}

		public Guide4D(Point4D basePoint, Vector4 point)
		{
			BasePoint = basePoint;
			Point = point;
			WidthController = new GuideWidthController(this);
			GuideController = new Guide4DController(this);
		}
	}
}

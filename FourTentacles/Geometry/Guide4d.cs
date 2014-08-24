using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourTentacles.Annotations;
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

				//todo: make for guide Changed prop
				BasePoint.Changed = true;
			}
		}

		public Guide4D(Point4D basePoint, Vector4 point)
		{
			BasePoint = basePoint;
			Point = point;
			WidthController = new GuideWidthController(this);
			GuideController = new Guide4DController(this);
		}

		[CanBeNull]
		public Guide4D GetSymmetricGuide()
		{
			return BasePoint.Guides.FirstOrDefault(g => g != this);
		}
	}
}

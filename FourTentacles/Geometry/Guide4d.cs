using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FourTentacles.Annotations;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Guide4D
	{
		public Point4D BasePoint;
		public Windrose WindRose;

		private readonly GuideWidthController widthController;
		private readonly PointWidthController pointWidthController;
		private readonly Guide4DController guideController;
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

		public bool Changed { get; set; }

		public Guide4D(Point4D basePoint, Vector4 point)
		{
			BasePoint = basePoint;
			Point = point;
			widthController = new GuideWidthController(this);
			guideController = new Guide4DController(this);
			pointWidthController = new PointWidthController(this);
			WindRose = new Windrose(point.Xyz);
		}


		public void DrawWidthCircle(RenderContext context, Vector3 pos, float width, bool selected)
		{
			var table = new SinCosTable(48);
			var pixOffset = (float)context.Camera.GetPerspectiveRatio(pos);
			width += Math.Sign(width) * pixOffset;

			Material.SetLineMaterial(selected ? Color.Red : Color.White);
			GL.Disable(EnableCap.DepthTest);
			GL.Begin(PrimitiveType.LineStrip);
			GL.Vertex3(WindRose.North * (pixOffset * 5 + width) + pos);
			foreach (Vector3 vec in table.Points(WindRose.North, WindRose.West))
				GL.Vertex3(vec * width + pos);
			GL.Vertex3(WindRose.North * width + pos);
			GL.End();
		}

		public IEnumerable<Controller> GetControllers()
		{
				yield return guideController;
				yield return widthController;
				yield return pointWidthController;
		}

		[CanBeNull]
		public Guide4D GetSymmetricGuide()
		{
			return BasePoint.Guides.FirstOrDefault(g => g != this);
		}
	}
}

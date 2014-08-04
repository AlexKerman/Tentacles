﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	class Point4D : Node
	{
		private const int PointSizePx = 4;

		public Windrose WindRose;
		private SinCosTable table;
		private Vector4 point;

		public readonly List<Guide4D> Guides = new List<Guide4D>();
		private PointWidthController WidthController;
		public bool Changed;

		public Vector4 Point
		{
			get { return point; }
			set
			{
				if(point == value) return;
				Changed = true;
				point = value;
			}
		}

		public Point4D(Vector4 point)
		{
			Point = point;
			WidthController = new PointWidthController(this);
		}

		public override Vector3 Pos
		{
			get { return Point.Xyz; }
			set { Point = new Vector4(value, Point.W); }
		}

		public override void Render(RenderContext context)
		{
			if (context.Mode == RenderMode.Selection)
			{
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(Vector3.Zero);
				GL.End();
			}
			else
			{
				DrawOrthoPoint(context, Pos);
			}
		}

		public void DrawOrthoPoint(RenderContext context, Vector3 vector)
		{
			Vector2 pos = context.WorldToScreen(vector);
			int x = (int)(pos.X - PointSizePx / 2.0f);
			int y = (int)(pos.Y - PointSizePx / 2.0f);

			Material.SetLineMaterial(IsSelected ? Color.Red : Color.White);

			GL.PushMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.MatrixMode(MatrixMode.Modelview);

			context.Camera.SetOrtho();
			GL.Disable(EnableCap.LineSmooth);

			GL.Begin(PrimitiveType.LineStrip);
			GL.Vertex2(x, y);
			GL.Vertex2(x + PointSizePx, y);
			GL.Vertex2(x + PointSizePx, y + PointSizePx);
			GL.Vertex2(x, y + PointSizePx);
			GL.Vertex2(x, y - 1);
			GL.End();

			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
		}

		public IEnumerable<Controller> GetControllers()
		{
			yield return WidthController;
			foreach (var guide in Guides)
			{
				yield return guide.GuideController;
				yield return guide.WidthController;
			}
		}

		public void DrawWidthCircle(RenderContext context, Vector4 pos)
		{
			Material.SetLineMaterial(Color.White);
			GL.Begin(PrimitiveType.LineLoop);
			foreach (Vector3 vec in table.Points(WindRose.North, WindRose.West))
				GL.Vertex3(vec * pos.W + pos.Xyz + Pos);
			GL.End();
		}

		public void SetRose(Windrose windRose, SinCosTable table)
		{
			this.WindRose = windRose;
			this.table = table;
		}
	}
}
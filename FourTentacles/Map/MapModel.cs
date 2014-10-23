using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles.Map
{
	public class MapModel : Geometry
	{
		public CoordConverter converter;

		public Dictionary<uint, MapNode> nodes = new Dictionary<uint, MapNode>();
		private List<MapWay> mapWays = new List<MapWay>();
		public override void Render(RenderContext context)
		{
			
			mapWays.Sort((m1, m2) => m1.zorder.CompareTo(m2.zorder));
			foreach (var mapWay in mapWays)
				mapWay.Render();
		}

		public override BoundingBox GetBoundingBox()
		{
			return converter.bbox;
		}

		public override int GetTrianglesCount()
		{
			return 0;
		}

		public void AddWay(List<MapNode> nodes, Dictionary<string, string> tags, uint id)
		{
			var way = CreateWay(tags, id);
			way.mapNodes = nodes;
			way.tags = tags;
			mapWays.Add(way);
		}

		private MapWay CreateWay(Dictionary<string, string> tags, uint id)
		{
			if (tags.ContainsKey("building")) 
				return new Building(id);

			if (tags.ContainsKey("landuse") && tags["landuse"] == "grass") return new Grass(id);

			return new MapWay(id);
		}
	}

	public class CoordConverter
	{
		private const double MapScale = 600000.0;

		private double centerLat;
		private double centerLon;

		public BoundingBox bbox = new BoundingBox();

		public CoordConverter(double minlat, double maxlat, double minlon, double maxlon)
		{
			centerLat = (minlat + maxlat)/2.0f;
			centerLon = (minlon + maxlon)/2.0f;
			
			bbox.Extend(new Vector3(Convert(minlat, minlon)));
			bbox.Extend(new Vector3(Convert(maxlat, maxlon)));
		}

		public Vector2 Convert(double lat, double lon)
		{
			return new Vector2((float) ((lat - centerLat)*MapScale), (float) ((lon - centerLon)*MapScale));
		}
	}

	public class MapNode
	{
		public uint id;
		public Vector2 coord;
		public Dictionary<string, string> tags = new Dictionary<string, string>();

		public MapNode(uint id, Vector2 coord)
		{
			this.id = id;
			this.coord = coord;
		}
	}

	public class MapWay
	{
		public int zorder = 0;

		public uint id;
		public List<MapNode> mapNodes = new List<MapNode>();
		public Dictionary<string,string>  tags = new Dictionary<string, string>();

		public MapWay(uint id)
		{
			this.id = id;
		}

		public virtual bool Filled
		{
			get
			{
				return mapNodes[0] == mapNodes[mapNodes.Count - 1];
			}
		}

		public virtual Color GetColor()
		{
			;
			if (tags.ContainsKey("highway")) return Color.DarkSlateGray;
			return Color.DarkBlue;
		}

		public virtual void Render()
		{
			GL.Disable(EnableCap.DepthTest);
			Material.SetLineMaterial(GetColor());
			GL.Begin(Filled ? PrimitiveType.Polygon : PrimitiveType.LineStrip);
			foreach (var node in mapNodes)
				GL.Vertex3(node.coord.X, 0, node.coord.Y);
			GL.End();

		}
	}

	public class Grass : MapWay
	{
		public Grass(uint id) : base(id)
		{
			zorder = 1;
		}

		public override Color GetColor()
		{
			return Color.Green;
		}
	}

	public class Building : MapWay
	{
		public Building(uint id) : base(id)
		{
			zorder = 10;
		}

		public override bool Filled
		{
			get { return true; }
		}

		public override Color GetColor()
		{
			return Color.SaddleBrown;
		}

		public override void Render()
		{
			int height = 1;
			if (tags.ContainsKey("building:levels"))
				height = Int32.Parse(tags["building:levels"]);

			height *= 30;

			Material.SetMeshMaterial();
			GL.Enable(EnableCap.DepthTest);
			GL.Color3(GetColor());

			GL.Begin(PrimitiveType.Quads);
			for (int i = 0; i < mapNodes.Count -1; i++)
			{
				var node1 = mapNodes[i];
				var node2 = mapNodes[i + 1];

				var ahead = new Vector3(node2.coord.X - node1.coord.X, 0, node2.coord.Y - node1.coord.Y);
				ahead.Normalize();
				Vector3 normal = Vector3.Cross(Vector3.UnitY, ahead);

				GL.Normal3(normal);
				GL.Vertex3(node1.coord.X, 0, node1.coord.Y);
				GL.Vertex3(node1.coord.X, height, node1.coord.Y);
				GL.Vertex3(node2.coord.X, height, node2.coord.Y);
				GL.Vertex3(node2.coord.X, 0, node2.coord.Y);
			}
			GL.End();

			GL.Normal3(Vector3.UnitY);
			GL.Begin(PrimitiveType.Polygon);
			foreach (var node in mapNodes)
				GL.Vertex3(node.coord.X, height, node.coord.Y);
			GL.End();
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FourTentacles.Map
{
	public partial class ImportOsmFile : Form
	{
		public MapModel Map = null;

		public ImportOsmFile()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
				ParseFile(openFileDialog1.FileName);
		}

		

		private void ParseFile(string fileName)
		{
			Map = new MapModel();
			XDocument xDoc;
			using (var sr = new StreamReader(fileName, Encoding.UTF8))
				xDoc = XDocument.Load(sr);

			IFormatProvider provider = new CultureInfo("en-US");

			var mapNodes = new Dictionary<uint, MapNode>();
			CoordConverter converter = null;

			foreach (var node in xDoc.Root.Nodes().OfType<XElement>())
			{
				switch (node.Name.ToString())
				{
					case "bounds":
						converter = new CoordConverter(
							Double.Parse(node.Attribute("minlat").Value, provider),
							Double.Parse(node.Attribute("maxlat").Value, provider),
							Double.Parse(node.Attribute("minlon").Value, provider),
							Double.Parse(node.Attribute("maxlon").Value, provider)
							);
						Map.converter = converter;
						break;

					case "node":
						var lat = Double.Parse(node.Attribute("lat").Value, provider);
						var lon = Double.Parse(node.Attribute("lon").Value, provider);
						var id = UInt32.Parse(node.Attribute("id").Value);
						var mapNode = new MapNode(id, converter.Convert(lat, lon));
						mapNodes.Add(id, mapNode);
						foreach (var tag in node.Nodes().OfType<XElement>())
							if (tag.Name == "tag")
								mapNode.tags.Add(tag.Attribute("k").Value, tag.Attribute("v").Value);
						break;
					case "way":
						uint wayId = UInt32.Parse(node.Attribute("id").Value);
						var wayNodes = new List<MapNode>();
						var wayTags = new Dictionary<string, string>();

						foreach (var tag in node.Nodes().OfType<XElement>())
						{
							if(tag.Name == "nd")
								wayNodes.Add(mapNodes[UInt32.Parse(tag.Attribute("ref").Value)]);

							if (tag.Name == "tag")
								wayTags.Add(tag.Attribute("k").Value, tag.Attribute("v").Value);
						}
						Map.AddWay(wayNodes, wayTags, wayId);
						break;
				}
			}
			Map.nodes = mapNodes;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using SpriteSheetPacker.Extensibility;

namespace SpriteSheetPacker.XnaExporters
{
	// writes out an XML file ready to be put into a XNA Content project and get compiled as content.
	// this file can be loaded using Content.Load<Dictionary<string, Rectangle>> from inside the game.
	[Export(typeof(IMapExporter))]
	public class XmlMapExporter : IMapExporter
	{
		public string Extension
		{
			get { return "xml"; }
		}

		public void Save(string filename, Dictionary<string, Rectangle> map)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				writer.WriteLine("<XnaContent>");
				writer.WriteLine("<Asset Type=\"System.Collections.Generic.Dictionary[System.String, Microsoft.Xna.Framework.Rectangle]\">");

				foreach (var entry in map)
				{
					Rectangle r = entry.Value;
					writer.WriteLine(string.Format(
						"<Item><Key>{0}</Key><Value>{1} {2} {3} {4}</Value></Item>", 
						Path.GetFileNameWithoutExtension(entry.Key), 
						r.X, 
						r.Y, 
						r.Width, 
						r.Height));
				}

				writer.WriteLine("</Asset>");
				writer.WriteLine("</XnaContent>");
			}
		}
	}
}

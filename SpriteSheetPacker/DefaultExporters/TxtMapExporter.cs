using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using SpriteSheetPacker.Extensibility;

namespace SpriteSheetPacker.DefaultExporters
{
	[Export(typeof(IMapExporter))]
	public class TxtMapExporter : IMapExporter
	{
		public string Extension
		{
			get { return "txt"; }
		}

		public void Save(string filename, Dictionary<string, Rectangle> map)
		{
			// copy the files list and sort alphabetically
			string[] keys = new string[map.Count];
			map.Keys.CopyTo(keys, 0);
			List<string> outputFiles = new List<string>(keys);
			outputFiles.Sort();

			using (StreamWriter writer = new StreamWriter(filename))
			{
				foreach (var image in outputFiles)
				{
					// get the destination rectangle
					Rectangle destination = map[image];

					// write out the destination rectangle for this bitmap
					writer.WriteLine(string.Format(
	                 	"{0} = {1} {2} {3} {4}", 
	                 	Path.GetFileNameWithoutExtension(image), 
	                 	destination.X, 
	                 	destination.Y, 
	                 	destination.Width, 
	                 	destination.Height));
				}
			}
		}
	}
}
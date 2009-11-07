using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using SpriteSheetPacker.Extensibility;

namespace SpriteSheetPacker.DefaultExporters
{
	[Export(typeof(IImageExporter))]
	public class PngImageExporter : IImageExporter
	{
		public string Extension
		{
			get { return "png"; }
		}

		public void Save(string filename, Bitmap image)
		{
			image.Save(filename, ImageFormat.Png);
		}
	}
}
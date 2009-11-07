using System.Drawing;

namespace SpriteSheetPacker.Extensibility
{
	/// <summary>
	/// An object able to save a sprite sheet image.
	/// </summary>
	public interface IImageExporter
	{
		/// <summary>
		/// Gets the extension for the image file type.
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Saves the image to a file.
		/// </summary>
		/// <param name="filename">The file to which the image should be saved.</param>
		/// <param name="image">The image to save to the file.</param>
		void Save(string filename, Bitmap image);
	}
}

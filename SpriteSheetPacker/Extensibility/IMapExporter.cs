using System.Collections.Generic;
using System.Drawing;

namespace SpriteSheetPacker.Extensibility
{
	/// <summary>
	/// An object able to save a sprite sheet map.
	/// </summary>
	public interface IMapExporter
	{
		/// <summary>
		/// Gets the extension for the map file type.
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Saves the map of rectangles to a file.
		/// </summary>
		/// <param name="filename">The file to which the map should be saved.</param>
		/// <param name="map">The map of the locations within the output image where each subimage is found. The strings are full file paths to the original images.</param>
		void Save(string filename, Dictionary<string, Rectangle> map);
	}
}
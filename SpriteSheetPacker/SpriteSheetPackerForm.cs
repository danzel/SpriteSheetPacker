﻿#region MIT License

/*
 * Copyright (c) 2009 Nick Gravelyn (nick@gravelyn.com), Markus Ewald (cygon@nuclex.org)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 * 
 */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SpriteSheetPacker
{
	internal partial class SpriteSheetPackerForm : Form
	{
		private static readonly Stopwatch stopWatch = new Stopwatch();

		private sspack.IImageExporter currentImageExporter;
		private sspack.IMapExporter currentMapExporter;

		// a list of the files we'll build into our sprite sheet
		private readonly List<string> files = new List<string>();

		public SpriteSheetPackerForm()
		{
			InitializeComponent();

			// set our open file dialog filter based on the valid extensions
			imageOpenFileDialog.Filter = "Image Files|";
			foreach (var ext in sspack.MiscHelper.AllowedImageExtensions) 
				imageOpenFileDialog.Filter += string.Format("*.{0};", ext);

			// set the UI values to our defaults
			maxWidthTxtBox.Text = sspack.Constants.DefaultMaximumSheetWidth.ToString();
			maxHeightTxtBox.Text = sspack.Constants.DefaultMaximumSheetHeight.ToString();
			paddingTxtBox.Text = sspack.Constants.DefaultImagePadding.ToString();
		}

		// configures our image save dialog to take into account all loaded image exporters
		public void GenerateImageSaveDialog()
		{
			string filter = "";
			foreach (var exporter in sspack.Exporters.ImageExporters)
				filter += string.Format("{0} Files|*.{1}|", exporter.ImageExtension.ToUpper(), exporter.ImageExtension);
			filter = filter.Remove(filter.Length - 1);

			imageSaveFileDialog.Filter = filter;
		}

		// configures our map save dialog to take into account all loaded map exporters
		public void GenerateMapSaveDialog()
		{
			string filter = "";
			foreach (var exporter in sspack.Exporters.MapExporters)
				filter += string.Format("{0} Files|*.{1}|", exporter.MapExtension.ToUpper(), exporter.MapExtension);
			filter = filter.Remove(filter.Length - 1);

			mapSaveFileDialog.Filter = filter;
		}

        private void AddFiles(IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				// if the path is a directory, add all files inside the directory
				if (Directory.Exists(path))
				{
					AddFiles(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
					continue;
				}

				// make sure the file is an image
				if (!sspack.MiscHelper.IsImageFile(path))
					continue;

				// prevent duplicates
				if (files.Contains(path)) 
					continue;

				// add to both our internal list and the list box
				files.Add(path);
				listBox1.Items.Add(path);
			}
		}

		// determines if a directory contains an image we can accept
		public static bool DirectoryContainsImages(string directory)
		{
			foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
			{
				if (sspack.MiscHelper.IsImageFile(file))
				{
					return true;
				}
			}

			return false;
		}

		private void listBox1_DragEnter(object sender, DragEventArgs e)
		{
			// if this drag is not for a file drop, ignore it
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) 
				return;

			// figure out if any the files being dropped are images
			bool imageFound = false;
			foreach (var f in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				// if the path is a directory and it contains images...
				if (Directory.Exists(f) && DirectoryContainsImages(f))
				{
					imageFound = true;
					break;
				}

				// or if the path itself is an image
				if (sspack.MiscHelper.IsImageFile(f))
				{
					imageFound = true;
					break;
				}
			}

			// if an image is being added, we're going to copy them. otherwise, we don't accept them.
			e.Effect = imageFound ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void listBox1_DragDrop(object sender, DragEventArgs e)
		{
			// add all the files dropped onto the list box
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
		}

		private void addImageBtn_Click(object sender, EventArgs e)
		{
			// show our file dialog and add all the resulting files
			if (imageOpenFileDialog.ShowDialog() == DialogResult.OK)
				AddFiles(imageOpenFileDialog.FileNames);
		}

		private void removeImageBtn_Click(object sender, EventArgs e)
		{
			// build a list of files to be removed
			List<string> filesToRemove = new List<string>();
			foreach (var i in listBox1.SelectedItems)
				filesToRemove.Add((string)i);

			// remove the files from our internal list
			files.RemoveAll(filesToRemove.Contains);

			// remove the files from our list box
			filesToRemove.ForEach(f => listBox1.Items.Remove(f));
		}

		private void clearBtn_Click(object sender, EventArgs e)
		{
			// clear both lists
			files.Clear();
			listBox1.Items.Clear();
		}

		private void browseImageBtn_Click(object sender, EventArgs e)
		{
			// show the file dialog
			imageSaveFileDialog.FileName = imageFileTxtBox.Text;
			if (imageSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the image path
				imageFileTxtBox.Text = imageSaveFileDialog.FileName;

				// figure out which image exporter to use based on the extension
				foreach (var exporter in sspack.Exporters.ImageExporters)
				{
					if (exporter.ImageExtension.Equals(Path.GetExtension(imageSaveFileDialog.FileName).Substring(1), StringComparison.InvariantCultureIgnoreCase))
					{
						currentImageExporter = exporter;
						break;
					}
				}
				
				// if there is no selected map exporter, default to the first map exporter
				if (currentMapExporter == null)
				{
					currentMapExporter = sspack.Exporters.MapExporters[0];
				}
				
				mapFileTxtBox.Text = imageSaveFileDialog.FileName.Remove(imageSaveFileDialog.FileName.Length - 3) + currentMapExporter.MapExtension.ToLower();
            }
		}

		private void browseMapBtn_Click(object sender, EventArgs e)
		{
			// show the file dialog
			mapSaveFileDialog.FileName = mapFileTxtBox.Text;
			if (mapSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the file path
				mapFileTxtBox.Text = mapSaveFileDialog.FileName;

				// figure out which map exporter to use based on the extension
				foreach (var exporter in sspack.Exporters.MapExporters)
				{
					if (exporter.MapExtension.Equals(Path.GetExtension(mapSaveFileDialog.FileName).Substring(1), StringComparison.InvariantCultureIgnoreCase))
					{
						currentMapExporter = exporter;
						break;
					}
				}

				// if there is no selected image exporter, default to the first image exporter
				if (currentImageExporter == null)
				{
					currentImageExporter = sspack.Exporters.ImageExporters[0];
				}

				imageFileTxtBox.Text = mapSaveFileDialog.FileName.Remove(mapSaveFileDialog.FileName.Length - 3) + currentImageExporter.ImageExtension.ToLower();
			}
		}

		private void buildBtn_Click(object sender, EventArgs e)
		{
			// check our parameters
			if (files.Count == 0)
			{
				ShowBuildError("No images to pack into sheet");
				return;
			}
			if (string.IsNullOrEmpty(imageFileTxtBox.Text))
			{
				ShowBuildError("No image filename given.");
				return;
			}
			if (string.IsNullOrEmpty(mapFileTxtBox.Text))
			{
				ShowBuildError("No text filename given.");
				return;
			}
			int outputWidth, outputHeight, padding;
			if (!int.TryParse(maxWidthTxtBox.Text, out outputWidth) || outputWidth < 1)
			{
				ShowBuildError("Maximum width is not a valid integer value greater than 0.");
				return;
			}
			if (!int.TryParse(maxHeightTxtBox.Text, out outputHeight) || outputHeight < 1)
			{
				ShowBuildError("Maximum height is not a valid integer value greater than 0.");
				return;
			}
			if (!int.TryParse(paddingTxtBox.Text, out padding) || padding < 0)
			{
				ShowBuildError("Image padding is not a valid non-negative integer");
				return;
			}

			// disable all the controls while the build happens
			foreach (Control control in Controls)
				control.Enabled = false;

			int mw = int.Parse(maxWidthTxtBox.Text);
			int mh = int.Parse(maxHeightTxtBox.Text);
			int pad = int.Parse(paddingTxtBox.Text);
			bool pow2 = powOf2CheckBox.Checked;
			bool square = squareCheckBox.Checked;
			bool generateMap = generateMapCheckBox.Checked;
			string image = imageFileTxtBox.Text;
			string map = mapFileTxtBox.Text;

			string fileList = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpriteSheetPacker");
			if (!Directory.Exists(fileList))
			{
				Directory.CreateDirectory(fileList);
			}
			fileList = Path.Combine(fileList, "FileList.txt");

			using (StreamWriter writer = new StreamWriter(fileList))
			{
				foreach (var file in files)
				{
					writer.WriteLine(file);
				}
			}

			// construct the arguments in a list so we can pass the array cleanly to sspack.Program.Launch()
			List<string> args = new List<string>();
			args.Add("/image:" + image);
			if (generateMap)
				args.Add("/map:" + map);
			args.Add("/mw:" + mw);
			args.Add("/mh:" + mh);
			args.Add("/pad:" + pad);
			if (pow2)
				args.Add("/pow2");
			if (square)
				args.Add("/sqr");
			args.Add("/il:" + fileList);

			int sspackResult = 0;
			Thread buildThread = new Thread(() => sspackResult = sspack.Program.Launch(args.ToArray()));

			stopWatch.Reset();
			stopWatch.Start();
			buildThread.Start();

			// wait for the thread to complete
			while (buildThread.IsAlive) { Thread.Sleep(5); }

			stopWatch.Stop();

			if (sspackResult == 0)
			{
#if DEBUG
				MessageBox.Show("Build completed in " + stopWatch.Elapsed.TotalSeconds + " seconds.");
#else
				MessageBox.Show("Build Complete", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
			}
			else
			{
				ShowBuildError("Error packing images: " + SpaceErrorCode((sspack.FailCode)sspackResult));
			}

			// re-enable all our controls
			foreach (Control control in Controls)
				control.Enabled = true;
		}

		private static string SpaceErrorCode(sspack.FailCode failCode)
		{
			string error = failCode.ToString();

			string result = error[0].ToString();

			for (int i = 1; i < error.Length; i++)
			{
				char c = error[i];
				if (char.IsUpper(c))
					result += " ";
				result += c;
			}

			return result;
		}

		private static void ShowBuildError(string error)
		{
			MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
	}
}
